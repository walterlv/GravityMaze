using System;
using System.Collections.Generic;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI;

namespace Walterlv.GravityMaze.Game.Framework
{
    public class GameObject : IGameObject
    {
        private IGameContext _context;
        private ICanvasResourceCreator _resourceCreator;
        private GameObject Parent { get; set; }
        private List<IGameObject> Children { get; } = new List<IGameObject>();

        public IGameContext Context
        {
            get => FindFromAncestor(o => o._context);
            set => _context = value;
        }

        public ICanvasResourceCreator ResourceCreator
        {
            get => FindFromAncestor(o => o._resourceCreator);
            set => _resourceCreator = value;
        }

        protected void AddChildren(params IGameObject[] gameObjects)
        {
            foreach (var @object in gameObjects)
            {
                if (@object is GameObject go) go.Parent = this;

                Children.Add(@object);
            }
        }

        protected void RemoveChildren(params IGameObject[] gameObjects)
        {
            foreach (var @object in gameObjects)
            {
                if (@object is GameObject go) go.Parent = null;
                if (@object is IGameObject igo) Children.Remove(igo);
            }
        }

        private T FindFromAncestor<T>(Func<GameObject, T> getField) where T : class
        {
            var value = getField(this);
            var parent = Parent;

            while (value == null && parent != null)
            {
                value = getField(parent);
                parent = parent.Parent;
            }

            return value;
        }

        public void Update(CanvasTimingInformation timing)
        {
            OnUpdate(timing);
            foreach (var child in Children) child.Update(timing);
        }

        public void Draw(CanvasDrawingSession ds)
        {
            OnDraw(ds);
            foreach (var child in Children) child.Draw(ds);
        }

        protected virtual void OnUpdate(CanvasTimingInformation timing)
        {
        }

        protected virtual void OnDraw(CanvasDrawingSession ds)
        {
        }
    }
}