using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Walterlv.GravityMaze.Game;
using Walterlv.GravityMaze.Game.Framework;

namespace Walterlv.GravityMaze
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();

            _input = new GameInput();
            _context = new GameContext
            {
                Input = _input,
            };
            _game = new MazeGame
            {
                Context = _context,
            };

            SizeChanged += OnSizeChanged;
            Window.Current.CoreWindow.KeyDown += OnKeyDown;
            Window.Current.CoreWindow.KeyUp += OnKeyUp;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            _context.SurfaceBounds = new Rect(0, 0, ActualWidth, ActualHeight);
        }

        private void OnKeyDown(CoreWindow sender, KeyEventArgs e)
        {
            switch (e.VirtualKey)
            {
                case VirtualKey.Left:
                    _input.Left = true;
                    break;
                case VirtualKey.Up:
                    _input.Up = true;
                    break;
                case VirtualKey.Right:
                    _input.Right = true;
                    break;
                case VirtualKey.Down:
                    _input.Down = true;
                    break;
            }
        }

        private void OnKeyUp(CoreWindow sender, KeyEventArgs e)
        {
            switch (e.VirtualKey)
            {
                case VirtualKey.Left:
                    _input.Left = false;
                    break;
                case VirtualKey.Up:
                    _input.Up = false;
                    break;
                case VirtualKey.Right:
                    _input.Right = false;
                    break;
                case VirtualKey.Down:
                    _input.Down = false;
                    break;
            }
        }

        private MazeGame _game;
        private GameContext _context;
        private GameInput _input;

        private void OnCreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs e)
        {
        }

        private void OnUpdate(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs e)
        {
            _game.Update(e.Timing);
        }

        private void OnDraw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs e)
        {
            using (var ds = e.DrawingSession)
            {
                _game.Draw(ds);
            }
        }
    }
}
