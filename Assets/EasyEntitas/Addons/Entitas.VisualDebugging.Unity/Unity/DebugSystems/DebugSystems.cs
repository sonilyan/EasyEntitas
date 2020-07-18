using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace Entitas.VisualDebugging.Unity {

    public enum AvgResetInterval {
        Always = 1,
        VeryFast = 30,
        Fast = 60,
        Normal = 120,
        Slow = 300,
        Never = int.MaxValue
    }

    public class DebugSystems : Systems {

        public static AvgResetInterval avgResetInterval = AvgResetInterval.Never;

        public int totalInitializeSystemsCount {
            get {
                var total = 0;
                foreach (var system in _initializeSystems) {
                    var debugSystems = system as DebugSystems;
                    total += debugSystems != null ? debugSystems.totalInitializeSystemsCount : 1;
                }
                return total;
            }
        }

        public int totalExecuteSystemsCount {
            get {
                var total = 0;
                foreach (var system in _executeSystems) {
                    var debugSystems = system as DebugSystems;
                    total += debugSystems != null ? debugSystems.totalExecuteSystemsCount : 1;
                }
                return total;
            }
        }

        public int totalCleanupSystemsCount {
            get {
                var total = 0;
                foreach (var system in _cleanupSystems) {
                    var debugSystems = system as DebugSystems;
                    total += debugSystems != null ? debugSystems.totalCleanupSystemsCount : 1;
                }
                return total;
            }
        }

        public int totalTearDownSystemsCount {
            get {
                var total = 0;
                foreach (var system in _tearDownSystems) {
                    var debugSystems = system as DebugSystems;
                    total += debugSystems != null ? debugSystems.totalTearDownSystemsCount : 1;
                }
                return total;
            }
        }

        public int totalSystemsCount {
            get {
                var total = 0;
                foreach (var system in _systems) {
                    var debugSystems = system as DebugSystems;
                    total += debugSystems != null ? debugSystems.totalSystemsCount : 1;
                }
                return total;
            }
        }

        public int initializeSystemsCount { get { return _initializeSystems.Count; } }
        public int executeSystemsCount { get { return _executeSystems.Count; } }
        public int cleanupSystemsCount { get { return _cleanupSystems.Count; } }
        public int tearDownSystemsCount { get { return _tearDownSystems.Count; } }

        public string name { get { return _name; } }
        public GameObject gameObject { get { return _gameObject; } }
        public SystemInfo systemInfo { get { return _systemInfo; } }

        public double executeDuration { get { return _executeDuration; } }
        public double cleanupDuration { get { return _cleanupDuration; } }

        public SystemInfo[] initializeSystemInfos { get { return _initializeSystemInfos.Values.ToArray(); } }
        public SystemInfo[] executeSystemInfos { get { return _executeSystemInfos.Values.ToArray(); } }
        public SystemInfo[] cleanupSystemInfos { get { return _cleanupSystemInfos.Values.ToArray(); } }
        public SystemInfo[] tearDownSystemInfos { get { return _tearDownSystemInfos.Values.ToArray(); } }

        public bool paused;

        string _name;

        List<ISystem> _systems;
        GameObject _gameObject;
        SystemInfo _systemInfo;

		Dictionary<IInitializeSystem, SystemInfo> _initializeSystemInfos;
        Dictionary<IExecuteSystem,SystemInfo> _executeSystemInfos;
		Dictionary<ICleanupSystem,SystemInfo> _cleanupSystemInfos;
		Dictionary<ITearDownSystem,SystemInfo> _tearDownSystemInfos;

        Stopwatch _stopwatch;

        double _executeDuration;
        double _cleanupDuration;

		protected DebugSystems(Systems systems) : base(systems) {
		}

		public DebugSystems(string name) {
            initialize(name);
        }

        protected DebugSystems(bool noInit) {
        }

        protected void initialize(string name) {
            _name = name;
            _gameObject = new GameObject(name);
            _gameObject.AddComponent<DebugSystemsBehaviour>().Init(this);

            _systemInfo = new SystemInfo(this);

            _systems = new List<ISystem>();
            _initializeSystemInfos = new Dictionary<IInitializeSystem, SystemInfo>();
            _executeSystemInfos = new Dictionary<IExecuteSystem, SystemInfo>();
			_cleanupSystemInfos = new Dictionary<ICleanupSystem, SystemInfo>();
			_tearDownSystemInfos = new Dictionary<ITearDownSystem, SystemInfo>();

			_stopwatch = new Stopwatch();
        }

        public override Systems Add(ISystem system) {
            _systems.Add(system);

            SystemInfo childSystemInfo;

            var debugSystems = system as DebugSystems;
            if (debugSystems != null) {
                childSystemInfo = debugSystems.systemInfo;
                debugSystems.gameObject.transform.SetParent(_gameObject.transform, false);
            } else {
                childSystemInfo = new SystemInfo(system);
            }

            childSystemInfo.parentSystemInfo = _systemInfo;

            if (childSystemInfo.isInitializeSystems) {
                _initializeSystemInfos.Add(system as IInitializeSystem, childSystemInfo);
            }
            if (childSystemInfo.isExecuteSystems || childSystemInfo.isReactiveSystems) {
                _executeSystemInfos.Add(system as IExecuteSystem, childSystemInfo);
            }
            if (childSystemInfo.isCleanupSystems) {
                _cleanupSystemInfos.Add(system as ICleanupSystem, childSystemInfo);
            }
            if (childSystemInfo.isTearDownSystems) {
                _tearDownSystemInfos.Add(system as ITearDownSystem, childSystemInfo);
            }

            return base.Add(system);
        }

        public void ResetDurations() {
            foreach (var systemInfo in _executeSystemInfos) {
                systemInfo.Value.ResetDurations();
            }

            foreach (var system in _systems) {
                var debugSystems = system as DebugSystems;
                if (debugSystems != null) {
                    debugSystems.ResetDurations();
                }
            }
        }

        public override void Initialize() {
			for (int i = 0; i < _initializeSystems.Count; i++) {
				var system = _initializeSystems[i];
		        SystemInfo systemInfo;
		        if (_initializeSystemInfos.Keys.Contains(system)) {
			        systemInfo = _initializeSystemInfos[system];
		        }
		        else {
					systemInfo = new SystemInfo(system);
					systemInfo.parentSystemInfo = _systemInfo;
					_initializeSystemInfos.Add(system,systemInfo);
				}
                if (systemInfo.isActive) {
                    _stopwatch.Reset();
                    _stopwatch.Start();
					system.Initialize();
                    _stopwatch.Stop();
                    systemInfo.initializationDuration = _stopwatch.Elapsed.TotalMilliseconds;
                }
            }
        }

        public override void Execute() {
            if (!paused) {
                StepExecute();
            }
        }

        public override void Cleanup() {
            if (!paused) {
                StepCleanup();
            }
        }

        public void StepExecute() {
            _executeDuration = 0;
            if (Time.frameCount % (int)avgResetInterval == 0) {
                ResetDurations();
            }

	        for (int i = 0; i < _executeSystems.Count; i++) {
		        var system = _executeSystems[i];

		        SystemInfo systemInfo;
				if (_executeSystemInfos.Keys.Contains(system)) {
					systemInfo = _executeSystemInfos[system];
				}
				else {
					systemInfo = new SystemInfo(system);
					systemInfo.parentSystemInfo = _systemInfo;
					_executeSystemInfos.Add(system, systemInfo);
				}
                if (systemInfo.isActive) {
                    _stopwatch.Reset();
                    _stopwatch.Start();
					system.Execute();
                    _stopwatch.Stop();
                    var duration = _stopwatch.Elapsed.TotalMilliseconds;
                    _executeDuration += duration;
                    systemInfo.AddExecutionDuration(duration);
                }
            }
        }

        public void StepCleanup() {
            _cleanupDuration = 0;
			for (int i = 0; i < _cleanupSystems.Count; i++) {
				var system = _cleanupSystems[i];

				SystemInfo systemInfo;
				if (_cleanupSystemInfos.Keys.Contains(system)) {
					systemInfo = _cleanupSystemInfos[system];
				}
				else {
					systemInfo = new SystemInfo(system);
					systemInfo.parentSystemInfo = _systemInfo;
					_cleanupSystemInfos.Add(system, systemInfo);
				}
                if (systemInfo.isActive) {
                    _stopwatch.Reset();
                    _stopwatch.Start();
					system.Cleanup();
                    _stopwatch.Stop();
                    var duration = _stopwatch.Elapsed.TotalMilliseconds;
                    _cleanupDuration += duration;
                    systemInfo.AddCleanupDuration(duration);
                }
            }
        }

        public override void TearDown() {
			for (int i = 0; i < _tearDownSystems.Count; i++) {
				var system = _tearDownSystems[i];
				SystemInfo systemInfo;
				if (_tearDownSystemInfos.Keys.Contains(system)) {
					systemInfo = _tearDownSystemInfos[system];
				}
				else {
					systemInfo = new SystemInfo(system);
					systemInfo.parentSystemInfo = _systemInfo;
					_tearDownSystemInfos.Add(system, systemInfo);
				}
                if (systemInfo.isActive) {
                    _stopwatch.Reset();
                    _stopwatch.Start();
					system.TearDown();
                    _stopwatch.Stop();
                    systemInfo.teardownDuration = _stopwatch.Elapsed.TotalMilliseconds;
                }
            }
        }
    }
}
