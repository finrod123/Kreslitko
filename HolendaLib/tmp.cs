using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Holenda
{
    public interface IMouseResponsive
    {
        void MouseMove(Point newPos);

        void MouseDown(Point pos, MouseButtonEventArgs e);

        void MouseUp(Point pos, MouseButtonEventArgs e);
    }

    public interface IKeyResponsive
    {
        void KeyDown(KeyboardEventArgs e);

        void KeyUp(KeyboardEventArgs e);
    }

    public delegate void GeometryModelChangedHandler(GeometryModel origin, GeometryChange change);

    public class ChangeRecorder
    {

    }

    public abstract class GeometryPresenter
    {
        public GeometryModel Model { get; set; }
    }

    public class GeometryChange
    {
        public GeometryModel Subject { get; set; }
    }

    public class PointMove : GeometryChange
    {
        private Point point;
        private Point point_2;
        private Point point_3;

        public System.Windows.Point From { get; set; }
        public System.Windows.Point To { get; set; }

        public PointMove()
        {
        }

        public PointMove(GeometryModel subject, System.Windows.Point from, System.Windows.Point to)
        {
            this.Subject = subject;
            this.From = from;
            this.To = to;
        }
    }

    public abstract class GeometryModel
    {
        public event GeometryModelChangedHandler Geometry_Changed;

        protected virtual void OnGeometryChanged(GeometryModel model, GeometryChange change)
        {
            if (Geometry_Changed != null)
            {
                Geometry_Changed(model, change);
            }
        }

        public GeometryPresenter Presenter { get; set; }

        public abstract void RevertChange(GeometryChange ch);
    }

    public abstract class Tool : IMouseResponsive, IKeyResponsive
    {
        Scene scene;

        public virtual void MouseMove(Point newPos) { }

        public virtual void MouseDown(Point pos, MouseButtonEventArgs e) { }

        public virtual void MouseUp(Point pos, MouseButtonEventArgs e) { }

        public virtual void KeyDown(KeyboardEventArgs e) { }

        public virtual void KeyUp(KeyboardEventArgs e) { }
    }

    public delegate void PointEventHandler(Point origin, double newX, double newY);

    public class Point : GeometryModel
    {
        private double _X;
        public double X
        {
            get { return _X; }
            set
            {
                if (_X != value)
                {
                    _X = value;
                    OnGeometryChanged(this, new PointMove());
                }
            }
        }

        private double _Y;
        public double Y
        {
            get { return _Y; }
            set
            {
                if (_Y != value)
                {
                    OnGeometryChanged(this, new PointMove(this, new System.Windows.Point(_X, _Y), new System.Windows.Point(_X, value)));
                    _Y = value;
                }
            }
        }

        public Point(double x, double y)
        {
            this._X = x;
            this._Y = y;
        }

        public override void RevertChange(GeometryChange ch)
        {

        }
    }

    public class Line : GeometryModel
    {
        public Point A { get; set; }
        public Point B { get; set; }

        private GeometryModelChangedHandler pointMovedDelegate;
        private void Point_Moved(GeometryModel origin, GeometryChange change)
        {
            OnGeometryChanged(this, new GeometryChange());
        }

        public Line()
        {
            pointMovedDelegate = new GeometryModelChangedHandler(Point_Moved);
            A.Geometry_Changed += pointMovedDelegate;
            B.Geometry_Changed += pointMovedDelegate;
        }

        public override void RevertChange(GeometryChange ch)
        {

        }
    }

    public class Fastener : Tool
    {
        Scene scene;

        bool drawing = false;
        Point heldPoint;

        public void MouseMove(System.Windows.Point newPos)
        {
            if (drawing)
            {
                heldPoint.X = newPos.X;
                heldPoint.Y = newPos.Y;
            }
        }

        public void MouseDown(System.Windows.Point pos, MouseButtonEventArgs e)
        {
            if (!drawing)
            {

            }
        }

        public void MouseUp(System.Windows.Point pos, MouseButtonEventArgs e)
        {

        }

    }

    public class GeometryModelNurse
    {

    }

    public class Scene : IMouseResponsive, IKeyResponsive
    {
        Dictionary<Type, GeometryModelNurse> nurseCache;

        Tool tool;

        public void MouseMove(Point newPos)
        {
            if (tool != null)
            {
                tool.MouseMove(newPos);
            }
        }

        public void MouseDown(Point pos, MouseButtonEventArgs e)
        {
            if (tool != null)
            {
                tool.MouseDown(pos, e);
            }
        }

        public void MouseUp(Point pos, MouseButtonEventArgs e)
        {
            if (tool != null)
            {
                tool.MouseUp(pos, e);
            }
        }

        public void KeyDown(KeyboardEventArgs e)
        {
            if (tool != null)
            {
                tool.KeyDown(e);
            }
        }

        public void KeyUp(KeyboardEventArgs e)
        {
            if (tool != null)
            {
                tool.KeyUp(e);
            }
        }

        public void AddGeometryModel(GeometryModel model)
        {
            GeometryModelNurse nurse;
            nurse = nurseCache[model.GetType()];
        }
    }
}