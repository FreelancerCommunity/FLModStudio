using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace HelixEngine
{
    /// <summary>
    ///   A control that manipulates the camera by mouse and keyboard gestures.
    /// </summary>
    public class CameraController : Border
    {
        /// <summary>
        ///   The camera property.
        /// </summary>
        public static readonly DependencyProperty CameraProperty = DependencyProperty.Register(
            "Camera",
            typeof(PerspectiveCamera),
            typeof(CameraController),
            new UIPropertyMetadata(null));

        /// <summary>
        ///   The inertia factor property.
        /// </summary>
        public static readonly DependencyProperty InertiaFactorProperty = DependencyProperty.Register(
            "InertiaFactor", typeof(double), typeof(CameraController), new UIPropertyMetadata(0.9));

        /// <summary>
        ///   The is pan enabled property.
        /// </summary>
        public static readonly DependencyProperty IsPanEnabledProperty = DependencyProperty.Register(
            "IsPanEnabled", typeof(bool), typeof(CameraController), new UIPropertyMetadata(true));

        /// <summary>
        ///   The is zoom enabled property.
        /// </summary>
        public static readonly DependencyProperty IsZoomEnabledProperty = DependencyProperty.Register(
            "IsZoomEnabled", typeof(bool), typeof(CameraController), new UIPropertyMetadata(true));

        /// <summary>
        ///   The show camera target property.
        /// </summary>
        public static readonly DependencyProperty ShowCameraTargetProperty =
            DependencyProperty.Register(
                "ShowCameraTarget", typeof(bool), typeof(CameraController), new UIPropertyMetadata(true));

        /// <summary>
        ///   The spin release time property.
        /// </summary>
        public static readonly DependencyProperty SpinReleaseTimeProperty =
            DependencyProperty.Register(
                "SpinReleaseTime", typeof(int), typeof(CameraController), new UIPropertyMetadata(200));

        /// <summary>
        ///   The camera mode property.
        /// </summary>
        public static readonly DependencyProperty CameraModeProperty = DependencyProperty.Register(
            "CameraMode", typeof(CameraMode), typeof(CameraController), new UIPropertyMetadata(CameraMode.Inspect));

        /// <summary>
        ///   The camera rotation mode property.
        /// </summary>
        public static readonly DependencyProperty CameraRotationModeProperty =
            DependencyProperty.Register(
                "CameraRotationMode",
                typeof(CameraRotationMode),
                typeof(CameraController),
                new UIPropertyMetadata(CameraRotationMode.Turntable));

        /// <summary>
        ///   The enabled property.
        /// </summary>
        public static readonly DependencyProperty EnabledProperty = DependencyProperty.Register(
            "Enabled", typeof(bool), typeof(CameraController), new UIPropertyMetadata(true));

        /// <summary>
        ///   The viewport property.
        /// </summary>
        public static readonly DependencyProperty ViewportProperty = DependencyProperty.Register(
            "Viewport", typeof(Viewport3D), typeof(CameraController), new PropertyMetadata(null, ViewportChanged));

        /// <summary>
        ///   The last tick.
        /// </summary>
        private long lastTick;

        /// <summary>
        ///   The pan speed.
        /// </summary>
        private Vector3D panSpeed;

        /// <summary>
        ///   The rotation speed.
        /// </summary>
        private Vector rotationSpeed;

        /// <summary>
        ///   The target adorner.
        /// </summary>
        private Adorner targetAdorner;

        /// <summary>
        ///   The zoom speed.
        /// </summary>
        private double zoomSpeed;

        private readonly Stopwatch _watch = new Stopwatch();
        private Point3D? lastPoint3D = new Point3D();
        private Point _lastPosition;
        //private Point3D? _mouseDownPoint3D;
        private Point _mouseDownPosition;

        private bool isPanning;
        private bool isRotating;
        private bool isZooming;

        public double RotateSensitivity { get; set; }
        public double PanSensitivity { get; set; }

        public delegate void SelectionChangedType(DependencyObject visual);
        public SelectionChangedType SelectionChanged;

        private void OnSelectionChanged(DependencyObject visual)
        {
            if (this.SelectionChanged != null)
                this.SelectionChanged(visual);
        }

        /// <summary>
        ///   Initializes static members of the <see cref="CameraController" /> class.
        /// </summary>
        public CameraController()
        {
            Background = Brushes.Transparent;
            SubscribeEvents();

            _watch.Start();
            lastTick = _watch.ElapsedTicks;
        }

        /// <summary>
        ///   Gets or sets InertiaFactor.
        /// </summary>
        public double InertiaFactor
        {
            get
            {
                return (double)this.GetValue(InertiaFactorProperty);
            }

            set
            {
                this.SetValue(InertiaFactorProperty, value);
            }
        }

        /// <summary>
        ///   Max duration of mouse drag to activate spin
        /// </summary>
        /// <remarks>
        ///   If the time between mouse down and mouse up is less than this value, spin is activated.
        /// </remarks>
        public int SpinReleaseTime
        {
            get
            {
                return (int)this.GetValue(SpinReleaseTimeProperty);
            }

            set
            {
                this.SetValue(SpinReleaseTimeProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether IsPanEnabled.
        /// </summary>
        public bool IsPanEnabled
        {
            get
            {
                return (bool)this.GetValue(IsPanEnabledProperty);
            }

            set
            {
                this.SetValue(IsPanEnabledProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether IsZoomEnabled.
        /// </summary>
        public bool IsZoomEnabled
        {
            get
            {
                return (bool)this.GetValue(IsZoomEnabledProperty);
            }

            set
            {
                this.SetValue(IsZoomEnabledProperty, value);
            }
        }

        /// <summary>
        ///   Show a target adorner when manipulating the camera.
        /// </summary>
        public bool ShowCameraTarget
        {
            get
            {
                return (bool)this.GetValue(ShowCameraTargetProperty);
            }

            set
            {
                this.SetValue(ShowCameraTargetProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets Camera.
        /// </summary>
        public PerspectiveCamera Camera
        {
            get
            {
                return (PerspectiveCamera)this.GetValue(CameraProperty);
            }

            set
            {
                this.SetValue(CameraProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets ZoomSensitivity.
        /// </summary>
        public double ZoomSensitivity
        {
            get
            {
                return (double)this.GetValue(ZoomSensitivityProperty);
            }

            set
            {
                this.SetValue(ZoomSensitivityProperty, value);
            }
        }

        /// <summary>
        ///   The zoom sensitivity property.
        /// </summary>
        public static readonly DependencyProperty ZoomSensitivityProperty =
            DependencyProperty.Register(
                "ZoomSensitivity", typeof(double), typeof(CameraController), new UIPropertyMetadata(1.0));

        /// <summary>
        ///   Gets or sets CameraRotationMode.
        /// </summary>
        public CameraRotationMode CameraRotationMode
        {
            get
            {
                return (CameraRotationMode)this.GetValue(CameraRotationModeProperty);
            }

            set
            {
                this.SetValue(CameraRotationModeProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets CameraMode.
        /// </summary>
        public CameraMode CameraMode
        {
            get
            {
                return (CameraMode)this.GetValue(CameraModeProperty);
            }

            set
            {
                this.SetValue(CameraModeProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets Viewport.
        /// </summary>
        public Viewport3D Viewport
        {
            get
            {
                return (Viewport3D)this.GetValue(ViewportProperty);
            }

            set
            {
                this.SetValue(ViewportProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether Enabled.
        /// </summary>
        public bool Enabled
        {
            get
            {
                return (bool)this.GetValue(EnabledProperty);
            }

            set
            {
                this.SetValue(EnabledProperty, value);
            }
        }

        /// <summary>
        ///   Gets a value indicating whether IsActive.
        /// </summary>
        public bool IsActive
        {
            get
            {
                return this.Enabled && this.Viewport != null && this.Camera != null;
            }
        }

        /// <summary>
        ///   The viewport changed.
        /// </summary>
        /// <param name="d"> The sender. </param>
        /// <param name="e"> The event arguments. </param>
        private static void ViewportChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CameraController)d).OnViewportChanged();
        }

        /// <summary>
        ///   The on viewport changed.
        /// </summary>
        private void OnViewportChanged()
        {
            if (Camera == null && Viewport != null)
                Camera = Viewport.Camera as PerspectiveCamera;
        }

        /// <summary>
        ///   The subscribe events.
        /// </summary>
        private void SubscribeEvents()
        {
            this.MouseMove += MouseMoveHandler;
            this.MouseDown += MouseDownHandler;
            this.MouseUp += MouseUpHandler;
            this.MouseWheel += OnMouseWheel;
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        /// <summary>
        ///   The un subscribe events.
        /// </summary>
        private void UnSubscribeEvents()
        {
            this.MouseMove -= MouseMoveHandler;
            this.MouseDown -= MouseDownHandler;
            this.MouseUp -= MouseUpHandler;
            this.MouseWheel -= OnMouseWheel;
            CompositionTarget.Rendering -= CompositionTarget_Rendering;
        }

        /// <summary>
        ///   The rendering event handler.
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e"> The event arguments. </param>
        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            // Time in seconds
            double time = 1.0 * (_watch.ElapsedTicks - this.lastTick) / Stopwatch.Frequency;
            this.lastTick = _watch.ElapsedTicks;
            OnTimeStep(time);
        }

        /// <summary>
        ///   The on time step.
        /// </summary>
        /// <param name="time"> The time. </param>
        private void OnTimeStep(double time)
        {
            // should be independent of time
            double factor = Math.Pow(this.InertiaFactor, time / 0.012);
            factor = this.Clamp(factor, 0.2, 1);

            if (this.rotationSpeed.LengthSquared > 0.1)
            {
                Rotate(this.rotationSpeed.X * time, this.rotationSpeed.Y * time);
                this.rotationSpeed *= factor;
            }

            if (Math.Abs(this.panSpeed.LengthSquared) > 0.0001)
            {
                Pan(this.panSpeed * time);
                this.panSpeed *= factor;
            }

            if (Math.Abs(this.zoomSpeed) > 0.1)
            {
                Zoom(this.zoomSpeed * time);
                this.zoomSpeed *= factor;
            }
        }

        /// <summary>
        ///   The clamp.
        /// </summary>
        /// <param name="factor"> The factor. </param>
        /// <param name="min"> The min. </param>
        /// <param name="max"> The max. </param>
        /// <returns> The clamp. </returns>
        private double Clamp(double factor, double min, double max)
        {
            if (factor < min)
            {
                return min;
            }

            if (factor > max)
            {
                return max;
            }

            return factor;
        }

        private void MouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            if (!Enabled) return;
            if (Viewport == null)
                throw new NullReferenceException("Viewport");

            _mouseDownPosition = e.GetPosition(this);

            var isDoubleClick = e.ClickCount == 2;
            var isControlKey = Keyboard.IsKeyDown(Key.LeftCtrl);
            var isShiftKey = Keyboard.IsKeyDown(Key.LeftShift);

            // reset camera
            if (e.ChangedButton == MouseButton.Middle && isDoubleClick)
                ResetCamera();

            // select visual
            if (e.ChangedButton == MouseButton.Left)
            {
                //Point3D point;
                //Vector3D normal;
                //DependencyObject visual;
                var visual = Viewport3DHelper.FindVisual(Viewport, _mouseDownPosition, isShiftKey);
                //    _mouseDownPoint3D = point;
                //else
                //    _mouseDownPoint3D = null;

                // select object
                if (visual != null)
                    OnSelectionChanged(visual);

                // change the 'lookat' point
                //if (_mouseDownPoint3D != null && CheckButton(e, MouseAction.ChangeLookAt))
                //    LookAt(_mouseDownPoint3D.Value, 0);
            }

            // zoom, pan, rotate
            isZooming = e.ChangedButton == MouseButton.Right && isControlKey;
            isRotating = e.ChangedButton == MouseButton.Right && !isControlKey;
            isPanning = e.ChangedButton == MouseButton.Middle && !isDoubleClick;

            if (isZooming || isPanning || isRotating)
            {
                // show the adorner in the middle
                ShowTargetAdorner(new Point(Viewport.ActualWidth * 0.5, Viewport.ActualHeight * 0.5));

                e.Handled = true;
                ((UIElement)sender).CaptureMouse();

                _lastPosition = _mouseDownPosition;
                if (isPanning)
                    lastPoint3D = UnProject(_mouseDownPosition, CameraTarget(), Camera.LookDirection);
            }
        }

        public Point3D CameraTarget()
        {
            return Camera.Position + Camera.LookDirection;
        }

        private void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            if (!IsActive) return;

            var element = (UIElement)sender;
            if (element.IsMouseCaptured)
            {
                Point point = e.MouseDevice.GetPosition(element);

                Vector delta = point - _lastPosition;
                _lastPosition = point;

                if (isRotating)
                    Rotate(delta.X, delta.Y);
                if (isZooming)
                    Zoom(delta.Y * 0.01);
                if (isPanning)
                {
                    Point3D? thisPoint3D = UnProject(point, CameraTarget(), Camera.LookDirection);
                    Vector3D delta3D = lastPoint3D.Value - thisPoint3D.Value;
                    lastPoint3D = thisPoint3D + delta3D;
                    Pan(delta3D);
                }

                e.Handled = true;
            }
        }

        private void Pan(Point3D point3D, Point position)
        {
            Point3D? nowPoint3D = UnProject(position, point3D, Camera.LookDirection);
            Pan(point3D - nowPoint3D.Value);
            Point newPosition = Project(point3D);
            Debug.Assert(newPosition == position);
        }

        private void MouseUpHandler(object sender, MouseButtonEventArgs e)
        {
            if (!Enabled) return;

            var element = (UIElement)sender;

            isRotating = false;
            isZooming = false;
            isPanning = false;

            if (element.IsMouseCaptured)
            {
                e.Handled = true;
                HideTargetAdorner();
                element.ReleaseMouseCapture();
            }
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;
            //Zoom(-e.Delta * 0.001);
            AddZoomForce(-e.Delta * 0.001);
        }

        /// <summary>
        ///   The reset camera.
        /// </summary>
        public void ResetCamera()
        {
            CameraHelper.Reset(Camera);
            lastPoint3D = new Point3D();
        }

        public void Zoom(double delta)
        {
            if (!IsZoomEnabled)
                return;

            bool alt = (Keyboard.IsKeyDown(Key.LeftAlt));
            CameraMode cm = CameraMode;
            if (alt)
                cm = (CameraMode)(((int)CameraMode + 1) % 2);

            switch (cm)
            {
                case CameraMode.Inspect:
                    Point3D target = Camera.Position + Camera.LookDirection;
                    Vector3D lookDirection = Camera.LookDirection * (1 + delta);
                    CameraHelper.LookAt(Camera, target, lookDirection, 0);
                    //Point3D target = Camera.Position + Camera.LookDirection;
                    //Camera.LookDirection *= (1 + delta);
                    //Camera.Position = target - Camera.LookDirection;
                    break;
                case CameraMode.WalkAround:
                    Camera.Position -= Camera.LookDirection * delta;
                    break;
                case CameraMode.FixedPosition:
                    double fov = Camera.FieldOfView;
                    fov *= (1 + delta);
                    if (fov < 3) fov = 3;
                    if (fov > 160) fov = 160;
                    Camera.FieldOfView = fov;
                    break;
            }
        }

        public void Pan(Vector3D delta)
        {
            if (!IsPanEnabled)
                return;
            if (CameraMode == CameraMode.FixedPosition)
                return;
            Camera.Position += delta;
        }

        public void Rotate(double dx, double dy)
        {
            PerspectiveCamera c = Camera;

            Point3D target = c.Position + c.LookDirection;

            // toggle rotation mode if the user presses alt
            bool alt = (Keyboard.IsKeyDown(Key.LeftAlt));

            if ((CameraRotationMode == CameraRotationMode.VirtualTrackball) != alt)
            {
                RotateRoam(dx, dy);
            }
            else
            {
                RotateTwoAxes(dx, dy);
            }

            if (Math.Abs(c.UpDirection.Length - 1) > 1e-8)
                c.UpDirection.Normalize();

            if (IsFixedPosition())
                c.Position = target - c.LookDirection;
        }

        public bool IsFixedPosition()
        {
            bool leftWinKey = Keyboard.IsKeyDown(Key.LWin);

            // fix the camera position if user presses left Windows key
            if (leftWinKey)
                return CameraMode != CameraMode.Inspect;

            return CameraMode == CameraMode.Inspect;
        }

        // http://www.codeplex.com/3DTools/Thread/View.aspx?ThreadId=22310
        private void RotateRoam(double dX, double dY)
        {
            PerspectiveCamera c = Camera;
            double dist = c.LookDirection.Length;

            Vector3D camZ = c.LookDirection;
            camZ.Normalize();
            Vector3D camX = -Vector3D.CrossProduct(camZ, c.UpDirection);
            camX.Normalize();
            Vector3D camY = Vector3D.CrossProduct(camZ, camX);
            camY.Normalize();

            var aarY = new AxisAngleRotation3D(camY, -dX * 0.5);
            var aarX = new AxisAngleRotation3D(camX, dY * 0.5);

            var rotY = new RotateTransform3D(aarY);
            var rotX = new RotateTransform3D(aarX);

            camZ = camZ * rotY.Value * rotX.Value;
            camZ.Normalize();
            camY = camY * rotX.Value * rotY.Value;
            camY.Normalize();

            Vector3D newLookDir = camZ * dist;
            Vector3D newUpDir = camY;

            Vector3D right = Vector3D.CrossProduct(newLookDir, newUpDir);
            right.Normalize();
            Vector3D modUpDir = Vector3D.CrossProduct(right, newLookDir);
            modUpDir.Normalize();
            if ((newUpDir - modUpDir).Length > 1e-8)
                newUpDir = modUpDir;

            c.LookDirection = newLookDir;
            c.UpDirection = newUpDir;
        }

        public void RotateTwoAxes(double dx, double dy)
        {
            PerspectiveCamera c = Camera;

            var up = new Vector3D(0, 0, 1);
            Vector3D dir = c.LookDirection;
            dir.Normalize();

            Vector3D right = Vector3D.CrossProduct(dir, c.UpDirection);
            right.Normalize();

            double d = -0.5;
            if (CameraMode == CameraMode.WalkAround)
                d = 0.1;

            var q1 = new Quaternion(up, d * dx);
            var q2 = new Quaternion(right, d * dy);
            Quaternion q = q1 * q2;

            var m = new Matrix3D();
            m.Rotate(q);

            Vector3D newLookDir = m.Transform(c.LookDirection);
            Vector3D newUpDir = m.Transform(c.UpDirection);

            right = Vector3D.CrossProduct(newLookDir, newUpDir);
            right.Normalize();
            Vector3D modUpDir = Vector3D.CrossProduct(right, newLookDir);
            modUpDir.Normalize();
            if ((newUpDir - modUpDir).Length > 1e-8)
                newUpDir = modUpDir;

            c.LookDirection = newLookDir;
            c.UpDirection = newUpDir;
        }

        /// <summary>
        ///   The add pan force.
        /// </summary>
        /// <param name="dx"> The dx. </param>
        /// <param name="dy"> The dy. </param>
        public void AddPanForce(double dx, double dy)
        {
            this.AddPanForce(this.FindPanVector(dx, dy));
        }

        /// <summary>
        ///   The find pan vector.
        /// </summary>
        /// <param name="dx"> The dx. </param>
        /// <param name="dy"> The dy. </param>
        /// <returns> </returns>
        private Vector3D FindPanVector(double dx, double dy)
        {
            PerspectiveCamera pc = Camera;
            Vector3D axis1 = Vector3D.CrossProduct(pc.LookDirection, pc.UpDirection);
            Vector3D axis2 = Vector3D.CrossProduct(axis1, pc.LookDirection);
            axis1.Normalize();
            axis2.Normalize();
            double l = pc.LookDirection.Length;
            double f = l * 0.001;
            Vector3D move = -axis1 * f * dx + axis2 * f * dy;

            // this should be dependent on distance to target?           
            return move;
        }

        /// <summary>
        ///   The add pan force.
        /// </summary>
        /// <param name="pan"> The pan. </param>
        public void AddPanForce(Vector3D pan)
        {
            if (!this.IsPanEnabled)
            {
                return;
            }

            this.panSpeed += pan * 40;
        }

        /// <summary>
        ///   The add rotate force.
        /// </summary>
        /// <param name="dx"> The dx. </param>
        /// <param name="dy"> The dy. </param>
        public void AddRotateForce(double dx, double dy)
        {
            this.rotationSpeed.X += dx * 40;
            this.rotationSpeed.Y += dy * 40;
        }

        /// <summary>
        ///   Adds the zoom force.
        /// </summary>
        /// <param name="dx"> The dx. </param>
        public void AddZoomForce(double dx)
        {
            if (!this.IsZoomEnabled)
            {
                return;
            }

            zoomSpeed += dx * 8;
            this.zoomSpeed += dx * 8;
        }

        /// <summary>
        /// Get the ray into the view volume given by the position in 2D (screen coordinates)
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Ray3D GetRay(Point position)
        {
            Point3D point1, point2;
            bool ok = Viewport3DHelper.Point2DtoPoint3D(Viewport, position, out point1, out point2);
            if (!ok)
                return null;

            return new Ray3D { Origin = point1, Direction = point2 - point1 };
        }

        /// <summary>
        /// Unproject a point from the screen (2D) to a point on plane (3D)
        /// </summary>
        /// <param name="p"></param>
        /// <param name="position">plane position</param>
        /// <param name="normal">plane normal</param>
        /// <returns></returns>
        public Point3D? UnProject(Point p, Point3D position, Vector3D normal)
        {
            Ray3D ray = GetRay(p);
            if (ray == null)
                return null;
            return ray.PlaneIntersection(position, normal);
        }

        /// <summary>
        /// Calculate the screen position of a 3D point
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public Point Project(Point3D p)
        {
            return Viewport3DHelper.Point3DtoPoint2D(Viewport, p);
        }

        public void Select(Point target, double animationTime)
        {
            PerspectiveCamera camera = Camera;
            if (camera == null) return;

            //Point ptMouse = args.GetPosition(viewport);
            HitTestResult result = VisualTreeHelper.HitTest(Viewport, target);

            //// We're only interested in 3D hits.
            //RayMeshGeometry3DHitTestResult result3d =
            //                    result as RayMeshGeometry3DHitTestResult;
        }

        /// <summary>
        ///   Shows the target adorner.
        /// </summary>
        /// <param name="position"> The position. </param>
        public void ShowTargetAdorner(Point position)
        {
            if (!this.ShowCameraTarget)
            {
                return;
            }

            if (this.targetAdorner != null)
            {
                return;
            }

            AdornerLayer myAdornerLayer = AdornerLayer.GetAdornerLayer(this.Viewport);
            this.targetAdorner = new TargetSymbolAdorner(this.Viewport, position);
            myAdornerLayer.Add(this.targetAdorner);
        }

        /// <summary>
        ///   Hides the target adorner.
        /// </summary>
        public void HideTargetAdorner()
        {
            AdornerLayer myAdornerLayer = AdornerLayer.GetAdornerLayer(this.Viewport);
            if (this.targetAdorner != null)
            {
                myAdornerLayer.Remove(this.targetAdorner);
            }

            this.targetAdorner = null;

            // the adorner sometimes leaves some 'dust', so refresh the viewport
            this.RefreshViewport();
        }

        /// <summary>
        ///   The refresh viewport.
        /// </summary>
        private void RefreshViewport()
        {
            // todo: this is a hack, should be improved

            // var mg = new ModelVisual3D { Content = new AmbientLight(Colors.White) };
            // Viewport.Children.Add(mg);
            // Viewport.Children.Remove(mg);
            Camera c = this.Viewport.Camera;
            this.Viewport.Camera = null;
            this.Viewport.Camera = c;

            // var w = Viewport.Width;
            // Viewport.Width = w-1;
            // Viewport.Width = w;

            // Viewport.InvalidateVisual();
        }
    }
}