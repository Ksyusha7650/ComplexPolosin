using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Mysqlx.Crud;

namespace ModelPolosin; 

public partial class AnimationWindow : Window {
    
     public class LambdaCollection<T> : Collection<T>
        where T : DependencyObject, new() {
        public LambdaCollection(int count) {
            while (count-- > 0) {
                Add(new T());
            } 
        }

        public LambdaCollection<T> WithProperty<U>(DependencyProperty property, Func<int, U> generator) {
            for (int i = 1; i < Count; i++) {
                this[i].SetValue(property, generator(i));
            }

            return this;
        }

        public LambdaCollection<T> WithPropertyRect<U>(DependencyProperty property, Func<int, U> generator) {
            this[0].SetValue(property, generator(0));
            return this;
        }

        public LambdaCollection<T> WithXY<U>(Func<int, U> xGen, Func<int, U> yGen) {
            for (int i = 0; i < Count; i++) {
                this[i].SetValue(Canvas.LeftProperty,  xGen(i - 2));
                this[i].SetValue(Canvas.TopProperty, yGen(i));
            }

            return this;
        }
        
    }

    public class LambdaDoubleAnimation : DoubleAnimation {
        public Func<double, double> ValueGenerator { get; set; }

        // protected override double GetCurrentValueCore(double defaultOriginValue, double defaultDestinationValue,
        //     AnimationClock animationClock) {
        //     return ValueGenerator(base.GetCurrentValueCore(defaultOriginValue, defaultDestinationValue, animationClock));
        // }
    }

    public class LambdaDoubleAnimationCollection : Collection<LambdaDoubleAnimation> {
        public LambdaDoubleAnimationCollection(int count, Func<int, double> from, Func<int, double> to, Func<int, 
            Duration> duration, Func<int, Func<double, double>> valueGenerator) {
            for (int i = 0; i < count; i++) {
                var lda = new LambdaDoubleAnimation {
                    From = from(i),
                    To = to(i),
                    Duration = duration(i),
                    //ValueGenerator = valueGenerator(i)
                };
                Add(lda);
            }
        }

        public void BeginApplyAnimation(UIElement[] targets, DependencyProperty property) {
            for (int i = 0; i < Count; i++) {
                targets[i].BeginAnimation(property, Items[i]);
            }
        }
    }
    
    private LambdaCollection<Ellipse> circles;
    private LambdaCollection<Rectangle> channel;
    private static double _height;
    private static double _length;
    private double _coverVelocity;
    private static int _count;
    private int _countOfRows;
        
    public class Ball {
        public Ellipse Shape { get; set; }
        public Point Position { get; set; }
        public double Radius { get; set; }
        public SolidColorBrush BallColor =  Brushes.DodgerBlue;
    }
    public class BallMovement {
        public Ball Ball { get; set; }
        public double Speed { get; set; }
    }
        
    private BallMovement[] balls;
    private BallMovement[] ballsNew;
    
    public AnimationWindow(double height, double length, double coverVelocity) {
        InitializeComponent();
        _height = height * 10000;
         // _length = length * 100;
        _length = 650;
        _coverVelocity = coverVelocity;
        HeightTextBox.Text = height.ToString();
        LengthTextBox.Text = length.ToString();
        CoverVelocityTextBox.Text = coverVelocity.ToString();
        
        _count = (int)(_length / 19);
        if (_count % 2 == 0) {
            _length = _count * 19 + 6;
        } else {
            _length = _count * 19 + 9;
        }

        // if (_count % 2 != 0) {
        //     _count -= 1;
        // }
        _countOfRows = (int)_height / 21;
        
        MyCanvas.Height = _height;
        MyCanvas.Width = _length;
        MyCanvas.Background = Brushes.Transparent;
        CanvasBorder.BorderBrush = Brushes.Black;
        CanvasBorder.Height = _height;
        CanvasBorder.Width = _length;
        DrawBalls();
        
        
    }

    private void DrawBalls() {
        balls = new BallMovement[_count * _countOfRows];
        int k = 0;
        for (int j = 0; j < _countOfRows; j++) {
            circles = new LambdaCollection<Ellipse>(_count)
                .WithProperty(WidthProperty, i => 10.0)
                .WithProperty(HeightProperty, i => 10.0)
                .WithProperty(Shape.FillProperty, i => new SolidColorBrush(Color.FromArgb(255, 135, 206, 250)))
                .WithXY(x => x * 20.0,
                    y => y * 0.0 + j * 20.0 - 23.0);
                
                
            int widthBetween = -60;
            foreach (var ellipse in circles) {
                //MyCanvas.Children.Add(ellipse);
     
                balls[k] = new BallMovement() {
                    Ball = new Ball() { Shape = ellipse, Position = new Point(widthBetween, j * 20.0 - 16.0), Radius = 5 },
                    /*Speed = 1 + j,*/
                    Speed = _coverVelocity / _height * (_height - j * 20),
                    //Angle = 45 // задаем направление движения шарика в градусах
                };
                Canvas.SetLeft(balls[k].Ball.Shape, widthBetween);
                MyCanvas.Children.Add(balls[k].Ball.Shape);
                widthBetween += 20;
                k++;
            }
        }
    }

    private Storyboard myStoryboard;
    
    private void MakeAnimation(BallMovement ball, bool isStarted = false) {
        var startPositionX = isStarted ? ball.Ball.Position.X : -58;
        var myDoubleAnimation = new DoubleAnimation {
                
            From = startPositionX,
            To = MyCanvas.Width - 50,
            FillBehavior = FillBehavior.Stop,
            Duration = new Duration(
                new TimeSpan((long)((MyCanvas.Width - 50 - startPositionX) * 30000))) // какое-то число для нормальных тиков
        };
        myDoubleAnimation.Completed += (o, args) => MyStoryboardOnCompleted(ball, myDoubleAnimation);
        Storyboard.SetTarget(myDoubleAnimation, ball.Ball.Shape);
        Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(Canvas.LeftProperty));
        myStoryboard = new Storyboard {
            SpeedRatio = ball.Speed / 10
        };
        myStoryboard.Children.Add(myDoubleAnimation);
        myStoryboard.Begin();
    }

    private void MyStoryboardOnCompleted(BallMovement ball, DependencyObject myDoubleAnimation) {
        MakeAnimation(ball);
    }
        
    private void ButtonDo_Click(object sender, RoutedEventArgs e) {
        if (ButtonDo.Content.ToString() == "Run") {
            ButtonDo.Content = "Stop";
            for (int i = 0; i < _count * (_countOfRows - 1); i++) {
                MakeAnimation(balls[i], true);
            }
        } else {
            ButtonDo.Content = "Run";
            
            MyCanvas.Children.Clear();
            for (int i = 0; i < _count * (_countOfRows - 1); i++) {
                
                myStoryboard.Remove(balls[i].Ball.Shape);
            }
            // foreach (var ball in balls) {
            //     myStoryboard.Remove(ball.Ball.Shape);
            // }
          
           DrawBalls();
        }
        // foreach (var ball in balls) {
        //     MakeAnimation(ball, true);
        // }

    }
    private readonly List<string> _incorrectValues = new();
    private bool CheckTextBox => _incorrectValues.Count == 0;
    public Color BorderColor = new() {
        A = 100
    };

    private void NumberValidationTextBox(object sender, TextCompositionEventArgs e) {
        var regex = new Regex("[^0-9.]+");
        e.Handled = regex.IsMatch(e.Text);
    }

    private void ZeroValidationTextBox(object sender, TextChangedEventArgs e) {
        var textBox = sender as TextBox;
        var exist = _incorrectValues.Any(incorrectTextBox => incorrectTextBox == textBox?.Name);
        if (textBox?.Text is "0" or "") {
            textBox.BorderBrush = Brushes.Red;
            if (!exist)
                _incorrectValues.Add(textBox.Name);
        } else {
            textBox.BorderBrush = new SolidColorBrush(BorderColor);
            if (exist)
                _incorrectValues.Remove(textBox.Name);
        }
    }
}