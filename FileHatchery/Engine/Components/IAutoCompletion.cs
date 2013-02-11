using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileHatchery.Engine.Components
{
    /// <summary>
    /// 자동완성 기능 구현 인터페이스
    /// </summary>
    public interface IAutoCompletion
    {
        List<string> Commands { get; }
    }
}
