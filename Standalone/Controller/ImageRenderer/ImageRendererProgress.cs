using System;
namespace Medical
{
    public interface ImageRendererProgress
    {
        void update(uint percentage, string status);
        bool Visible { get; set; }
    }
}
