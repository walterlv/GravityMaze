using System.Collections.Generic;
using Microsoft.Graphics.Canvas;

namespace Walterlv.GravityMaze.Game.Framework
{
    public class GameObject : IGameObject
    {
        private IGameContext _context;
        private GameObject Parent { get; set; }
        private List<IGameObject> Children { get; } = new List<IGameObject>();

        public IGameContext Context
        {
            get
            {
                var context = _context;
                var parent = Parent;

                while (context == null && parent != null)
                {
                    context = parent.Context;
                    parent = parent.Parent;
                }

                return context;
            }
            set => _context = value;
        }

        protected void AddChildren(params IGameObject[] gameObjects)
        {
            foreach (var @object in gameObjects)
            {
                if (@object is GameObject go)
                {
                    go.Parent = this;
                }

                Children.Add(@object);
            }
        }

        public void Draw(CanvasDrawingSession ds)
        {
            OnDraw(ds);
            foreach (var child in Children)
            {
                child.Draw(ds);
            }
        }

        protected virtual void OnDraw(CanvasDrawingSession ds)
        {
        }
    }
}
