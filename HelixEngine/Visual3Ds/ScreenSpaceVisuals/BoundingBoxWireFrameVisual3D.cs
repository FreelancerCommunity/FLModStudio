﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BoundingBoxWireFrameVisual3D.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixEngine
{
    using System;
    using System.Windows;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// A visual element that shows a wireframe for the specified bounding box.
    /// </summary>
    public class BoundingBoxWireFrameVisual3D : LinesVisual3D
    {
        /// <summary>
        /// Identifies the <see cref="BoundingBox"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BoundingBoxProperty = DependencyProperty.Register(
            "BoundingBox", typeof(Rect3D), typeof(BoundingBoxWireFrameVisual3D), new UIPropertyMetadata(new Rect3D(), BoxChanged));

        /// <summary>
        /// Gets or sets the bounding box.
        /// </summary>
        /// <value> The bounding box. </value>
        public Rect3D BoundingBox
        {
            get
            {
                return (Rect3D)this.GetValue(BoundingBoxProperty);
            }

            set
            {
                this.SetValue(BoundingBoxProperty, value);
            }
        }

        /// <summary>
        /// Updates the box.
        /// </summary>
        protected virtual void OnBoxChanged()
        {
            this.Points.Clear();
            if (this.BoundingBox.IsEmpty)
            {
                return;
            }

            Rect3D bb = this.BoundingBox;

            var p0 = new Point3D(bb.X, bb.Y, bb.Z);
            var p1 = new Point3D(bb.X, bb.Y + bb.SizeY, bb.Z);
            var p2 = new Point3D(bb.X + bb.SizeX, bb.Y + bb.SizeY, bb.Z);
            var p3 = new Point3D(bb.X + bb.SizeX, bb.Y, bb.Z);
            var p4 = new Point3D(bb.X, bb.Y, bb.Z + bb.SizeZ);
            var p5 = new Point3D(bb.X, bb.Y + bb.SizeY, bb.Z + bb.SizeZ);
            var p6 = new Point3D(bb.X + bb.SizeX, bb.Y + bb.SizeY, bb.Z + bb.SizeZ);
            var p7 = new Point3D(bb.X + bb.SizeX, bb.Y, bb.Z + bb.SizeZ);

            Action<Point3D, Point3D> AddEdge = (p, q) =>
            {
                this.Points.Add(p);
                this.Points.Add(q);
            };

            AddEdge(p0, p1);
            AddEdge(p1, p2);
            AddEdge(p2, p3);
            AddEdge(p3, p0);

            AddEdge(p4, p5);
            AddEdge(p5, p6);
            AddEdge(p6, p7);
            AddEdge(p7, p4);

            AddEdge(p0, p4);
            AddEdge(p1, p5);
            AddEdge(p2, p6);
            AddEdge(p3, p7);
        }

        /// <summary>
        /// Called when the box dimensions changed.
        /// </summary>
        /// <param name="d">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private static void BoxChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BoundingBoxWireFrameVisual3D)d).OnBoxChanged();
        }
    }
}