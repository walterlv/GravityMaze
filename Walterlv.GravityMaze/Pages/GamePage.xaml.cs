using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using Walterlv.GravityMaze.Game;
using Walterlv.GravityMaze.Game.Framework;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Walterlv.GravityMaze.Pages
{
    public sealed partial class GamePage : Page
    {
        public GamePage()
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
            var min = Math.Min(ActualWidth, ActualHeight);
            _context.SurfaceBounds =
                new Rect((ActualWidth - min) / 2 + 1, (ActualHeight - min) / 2 + 1, min - 2, min - 2);
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
                case VirtualKey.Escape:
                    UIPanel.Visibility = Visibility.Visible;
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

        private async void OnThumbnailSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var image = (Image) ((ListView) sender).SelectedItem;
            if (image.Source is BitmapImage bi)
            {
                var uri = bi.UriSource;
                var actual = uri.AbsoluteUri.Replace(".thumbnail.png", ".jpg");
                var actualUri = new Uri(actual);

                var boardMaterial = await CanvasBitmap.LoadAsync(GameCanvas, actualUri);
                _game.Board.Material = boardMaterial;
                UIPanel.Visibility = Visibility.Collapsed;
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