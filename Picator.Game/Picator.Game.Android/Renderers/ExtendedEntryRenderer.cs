using Android.Content;
using Picator.Game.Controls;
using Picator.Game.Droid.Renderers;
using System;
using System.ComponentModel;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ExtendedEntry), typeof(ExtendedEntryRenderer))]
namespace Picator.Game.Droid.Renderers
{
    public class ExtendedEntryRenderer : EntryRenderer
    {
        public ExtendedEntryRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            if (e.NewElement == null) return;
            Control.Background = null;
            SetIcon((ExtendedEntry)Element);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName.Equals(nameof(ExtendedEntry.Icon)))
            {
                SetIcon((ExtendedEntry)Element);
            }
        }

        private void SetIcon(ExtendedEntry view)
        {
            if (!string.IsNullOrEmpty(view.Icon))
            {
                try
                {
                    var context = Android.App.Application.Context;
                    var resId = context.Resources.GetIdentifier(Path.GetFileNameWithoutExtension(view.Icon), "drawable", context.PackageName);
                    if (resId != 0)
                        Control?.SetCompoundDrawablesWithIntrinsicBounds(0, 0, resId, 0);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                Control?.SetCompoundDrawablesWithIntrinsicBounds(0, 0, 0, 0);
            }
        }
    }
}