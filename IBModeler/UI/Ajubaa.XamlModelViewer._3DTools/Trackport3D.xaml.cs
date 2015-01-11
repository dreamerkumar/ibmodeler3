//---------------------------------------------------------------------------
//
// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Limited Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/limitedpermissivelicense.mspx
// All other rights reserved.
//
// This file is part of the 3D Tools for Windows Presentation Foundation
// project.  For more information, see:
// 
// http://CodePlex.com/Wiki/View.aspx?ProjectName=3DTools
//
// The following article discusses the mechanics behind this
// trackball implementation: http://viewport3d.com/trackball.htm
//
// Reading the article is not required to use this sample code,
// but skimming it might be useful.
//
//---------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using Ajubaa.Common._3DModelView;

namespace Ajubaa.XamlModelViewer._3DTools
{
    /// <summary>
    ///     Trackport3D loads a Model3D from a xaml file and displays it.  The user
    ///     may rotate the view by dragging the mouse with the left mouse button.
    ///     Dragging with the right mouse button will zoom in and out.
    /// 
    ///     Trackport3D is primarily an example of how to use the Trackball utility
    ///     class, but it may be used as a custom control in your own applications.
    /// </summary>
    public partial class Trackport3D
    {
        #region fields
        private readonly Trackball _trackball = new Trackball();
        private readonly ScreenSpaceLines3D _wireframe = new ScreenSpaceLines3D();
        private ViewMode _viewMode;
        private Model3D _model;
        #endregion

        #region properties
        public ModelData ModelData { get; set; }
        
        public Color HeadlightColor
        {
            get { return Headlight.Color; }
            set { Headlight.Color = value; }
        }

        public Color AmbientLightColor
        {
            get { return AmbientLight.Color; }
            set { AmbientLight.Color = value; }
        }

        public ViewMode ViewMode
        {
            get { return _viewMode; }
            set
            {
                _viewMode = value;
                SetupScene();
            }
        }
        #endregion 

        /// <summary>
        /// constructor
        /// </summary>
        public Trackport3D()
        {
            InitializeComponent();
            Viewport.Children.Add(_wireframe);
        }

        /// <summary>
        /// updates the model display    
        /// </summary>
        public void UpdateModelDisplay()
        {
            _model = ModelData.Model3DGroupDataForDisplay;

            //added by vk on 15-june-2010
            //this moves the object so that it's center is at the origin
            //this way, when we rotate the camera, which is always pointing to the origin, we get a better experience
            _model.Transform = new TranslateTransform3D(-ModelData.CenterPt.X, -ModelData.CenterPt.Y, -ModelData.CenterPt.Z);

            SetupScene();
            InitializeEverything();
        }

        /// <summary>
        /// Initializes rotation and scale transforms
        /// </summary>
        public void InitializeEverything()
        {
            //initialize the trackball properties
            _trackball.InitializeEverything();

            //reassign the camera and headlight properties after the trackball is reinitialized
            Camera.Transform = _trackball.Transform;
            Headlight.Transform = _trackball.Transform;
        }

        private void SetupScene()
        {
            switch (ViewMode)
            {
                case ViewMode.Solid:
                    Root.Content = _model;
                    _wireframe.Points.Clear();
                    CreateButton.Visibility = Visibility.Visible;
                    break;

                case ViewMode.Wireframe:
                    Root.Content = null;
                    _wireframe.MakeWireframe(_model);
                    CreateButton.Visibility = Visibility.Collapsed;
                    break;
            }
            SetupCamera();
        }

        void SetupCamera()
        {
            Camera.Position = new Point3D(0, 0, ModelData.MaximumSideLength*2.5);
            Camera.LookDirection = new Vector3D(0,0,-1);
            
            //doing the below caused problem because the rotate on mouse move logic is based on the assumption that camera always points to the origin
            //var objectPosition = ModelData.CenterPt;
            //var cameraPosition = objectPosition;
            //cameraPosition.Z  += ModelData.MaximumSideLength*2;
            //Camera.Position = cameraPosition;
            //Camera.LookDirection = new Vector3D(objectPosition.X - Camera.Position.X, objectPosition.Y - Camera.Position.Y, objectPosition.Z - Camera.Position.Z);
         }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Viewport3Ds only raise events when the mouse is over the rendered 3D geometry.
            // In order to capture events whenever the mouse is over the client are we use a
            // same sized transparent Border positioned on top of the Viewport3D.
            _trackball.EventSource = CaptureBorder;

            SetSolidViewMode();
        }

        public void SetWireframeMode()
        {
            ViewMode = ViewMode.Wireframe;
            BorderWithBackgrndColor.Background = Brushes.DarkBlue;
        }

        public void SetSolidViewMode()
        {
            ViewMode = ViewMode.Solid;
            BorderWithBackgrndColor.Background = null;
            ParentGrid.Background = CreateBackgroundBrush();
        }

        public Brush CreateBackgroundBrush()
        {
            // Create a LinearGradientBrush 
            var linearGradientBrush = new LinearGradientBrush();
            linearGradientBrush.GradientStops.Add(new GradientStop(Colors.White, 0.0));
            linearGradientBrush.GradientStops.Add(new GradientStop(Colors.SlateGray, 1.0));

            return linearGradientBrush;

        }

        private void WireFrameOrSolid(object sender, RoutedEventArgs e)
        {
            if (ViewMode == ViewMode.Solid)
            {
                SetWireframeMode();
                WireFrameOrSolidButton.Content = "Solid View";
            }
            else
            {
                SetSolidViewMode();
                WireFrameOrSolidButton.Content = "Wireframe View";
            }
        }

        #region animations

        #region moving view
        private PerspectiveCamera _origCameraBeforeMovingView;
        public void MovingView(object sender, RoutedEventArgs e)
        {
            _origCameraBeforeMovingView = Camera;
            var newCamera = _origCameraBeforeMovingView.Clone();

            #region set new camera animation
            Viewport.Camera = newCamera;

            const int duration = 3;

            var currentPosition = newCamera.Position;

            var nextPosition = new Point3D { X = currentPosition.X, Y = currentPosition.Y + 5, Z = currentPosition.Z};
            var positionAnimation = new Point3DAnimation(currentPosition, nextPosition, TimeSpan.FromSeconds(duration));
            positionAnimation.Completed += PositionAnimationCompleted;

            var currentLookDirection = new Vector3D(0,-5,-1);
            var nextLookDirection = new Vector3D(-nextPosition.X, -nextPosition.Y, -nextPosition.Z);
            var lookAnimation = new Vector3DAnimation(currentLookDirection, nextLookDirection, TimeSpan.FromMilliseconds(duration));
            
            var transform3DGroup = new Transform3DGroup(); 
            var rotateTransform3D1 = new RotateTransform3D(); 
            var axisAngleRotation3D1 = new AxisAngleRotation3D(new Vector3D(0, 1, 0), 0.0 ); 
            rotateTransform3D1.Rotation = axisAngleRotation3D1;  
            transform3DGroup.Children.Add(rotateTransform3D1);
            newCamera.Transform = transform3DGroup; 

            var tl = new DoubleAnimation(360, TimeSpan.FromSeconds(duration));

            axisAngleRotation3D1.BeginAnimation(AxisAngleRotation3D.AngleProperty, tl);
            newCamera.BeginAnimation(PerspectiveCamera.PositionProperty, positionAnimation);
            newCamera.BeginAnimation(PerspectiveCamera.LookDirectionProperty, lookAnimation);
            #endregion
        }
        private void PositionAnimationCompleted(object sender, EventArgs e)
        {
            Thread.Sleep(1000);
            Viewport.Camera = _origCameraBeforeMovingView;
        }
        #endregion
        
        #region slow generation
        private static BackgroundWorker _slowGenerationWorkerThread;
        private Int32Collection _origTriangleIndices;
        private Model3D _modelDuringSlowGeneration;
        private const int SlowGenerationTotalTimeInMilliseconds = 60;

        private void SlowGeneration(object sender, RoutedEventArgs e)
        {
            if (ViewMode != ViewMode.Solid)
                return;

            _modelDuringSlowGeneration = _model.Clone();

            var mesh = GetMesh(_modelDuringSlowGeneration);
            if (mesh == null)
                return;
            _origTriangleIndices = mesh.TriangleIndices.Clone();

            mesh.TriangleIndices.Clear();
            _slowGenerationWorkerThread = new BackgroundWorker
            {
                WorkerReportsProgress = true
            };
            _slowGenerationWorkerThread.DoWork+=SlowGenerationWorkerThreadDoWork;
            _slowGenerationWorkerThread.ProgressChanged+=SlowGenerationWorkerThreadProgressChanged;
            _slowGenerationWorkerThread.RunWorkerCompleted+= SlowGenerationWorkerThreadRunWorkerCompleted;

            Root.Content = _modelDuringSlowGeneration;
            
            _slowGenerationWorkerThread.RunWorkerAsync(_origTriangleIndices.Count);
        }
        private static void SlowGenerationWorkerThreadDoWork(object sender, DoWorkEventArgs e)
        {
            for (var i = 0; i < SlowGenerationTotalTimeInMilliseconds; i++)
            {
                Thread.Sleep(50);
                _slowGenerationWorkerThread.ReportProgress(i);
            }
            Thread.Sleep(1000);
        }
        private void SlowGenerationWorkerThreadProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var currentMesh = GetMesh(_modelDuringSlowGeneration);
            if (currentMesh == null)
                return;
            var currentCount = currentMesh.TriangleIndices.Count;
            var totalCount = _origTriangleIndices.Count;

            var idealAdditions = (int)totalCount / SlowGenerationTotalTimeInMilliseconds;
            if (idealAdditions <= 0)
                idealAdditions = 1;

            var toAdd = currentCount + idealAdditions < totalCount ? idealAdditions : totalCount - currentCount;

            var startIndex = currentMesh.TriangleIndices.Count;

            var endIndex = startIndex + toAdd - 1;

            for (var ctr = startIndex; ctr <= endIndex; ctr++)
            {
                currentMesh.TriangleIndices.Add(_origTriangleIndices[ctr]);
            }

            var rotateTransform3D1 = new RotateTransform3D();
            var axisAngleRotation3D1 = new AxisAngleRotation3D(new Vector3D(0, 1, 0),
                45.0 * e.ProgressPercentage / SlowGenerationTotalTimeInMilliseconds);
            rotateTransform3D1.Rotation = axisAngleRotation3D1;
            var scaleTransform = new ScaleTransform3D(.75,.75,.75,0,0,0);
            var transformGroup = new Transform3DGroup();
            transformGroup.Children.Add(scaleTransform);
            transformGroup.Children.Add(rotateTransform3D1);
            _modelDuringSlowGeneration.Transform = transformGroup;
        }
        private void SlowGenerationWorkerThreadRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Root.Content = _model;
        }
        private static MeshGeometry3D GetMesh(Model3D model3D)
        {
            var group = model3D as Model3DGroup;
            if (group == null || group.Children == null || group.Children.Count != 1)
                return null;
            var actualModel = group.Children[0];
            var geometryModel = actualModel as GeometryModel3D;
            if (geometryModel == null)
                return null;
            var mesh = geometryModel.Geometry as MeshGeometry3D;
            return mesh;
        }
        #endregion

        #region disappear
        private static BackgroundWorker _disappearWorkerThread;
        private const int DisappearIterationCount = 100;

        private void Disappear(object sender, RoutedEventArgs e)
        {
            _disappearWorkerThread = new BackgroundWorker{WorkerReportsProgress = true};
            _disappearWorkerThread.DoWork+=DisappearWorkerThreadDoWork; ;
            _disappearWorkerThread.ProgressChanged +=DisappearWorkerThreadProgressChanged; 
            _disappearWorkerThread.RunWorkerCompleted+=DisappearWorkerThreadRunWorkerCompleted;

            _disappearWorkerThread.RunWorkerAsync();
        }
        private static void DisappearWorkerThreadDoWork(object sender, DoWorkEventArgs e)
        {
            for (var i = 1; i < DisappearIterationCount; i++)
            {
                Thread.Sleep(40);
                _disappearWorkerThread.ReportProgress(i);
            }
        }
        private void DisappearWorkerThreadProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Viewport.Opacity = 1.0 - ((double)e.ProgressPercentage / DisappearIterationCount);
        }
        private void DisappearWorkerThreadRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Viewport.Opacity = 1.0 ;
        }
        #endregion 

        #endregion 
    }
}