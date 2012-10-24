using System;
namespace FileHatchery.Engine
{
    interface IExceptionHandler
    {
        void handleException(Exception E);
    }
}
