using System;
using Xamarin.Forms;

namespace XamDrawingView.Views.DrawingView;

/// <summary>
/// Contains last drawing line
/// </summary>
public class DrawingPointAddedEventArgs : EventArgs
{
	/// <summary>
	/// Last point
	/// </summary>
	public Point? Point { get; }

	/// <summary>
	/// Initializes last drawing line
	/// </summary>
	/// <param name="point">Last drawing line</param>
	public DrawingPointAddedEventArgs(Point point)
	{
		Point = point;
	}
}