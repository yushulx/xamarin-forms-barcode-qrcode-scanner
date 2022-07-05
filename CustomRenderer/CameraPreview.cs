﻿using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace CustomRenderer
{
    public class ResultReadyEventArgs : EventArgs
    {
        public ResultReadyEventArgs(object result)
        {
            Result = result;
        }

        public object Result { get; private set; }

    }

    public class CameraPreview : View
    {
        public static readonly BindableProperty CameraProperty = BindableProperty.Create(
            propertyName: "Camera",
            returnType: typeof(CameraOptions),
            declaringType: typeof(CameraPreview),
            defaultValue: CameraOptions.Rear);

        public CameraOptions Camera
        {
            get { return (CameraOptions)GetValue(CameraProperty); }
            set { SetValue(CameraProperty, value); }
        }

        public event EventHandler<ResultReadyEventArgs> ResultReady;

        public void NotifyResultReady(object result)
        {
            if (ResultReady != null)
            {
                ResultReady(this, new ResultReadyEventArgs(result));
            }
        }
    }
}