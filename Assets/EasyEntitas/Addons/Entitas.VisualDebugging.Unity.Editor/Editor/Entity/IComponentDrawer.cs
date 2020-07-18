using System;

namespace Entitas.VisualDebugging.Unity.Editor {

    public interface IComponentDrawer {

        bool HandlesType(Type type);

        IComponentBase DrawComponent(IComponentBase component);
    }
}
