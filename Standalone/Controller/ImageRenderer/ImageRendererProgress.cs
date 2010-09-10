using System;
namespace Medical
{
    interface ImageRendererProgress
    {
        void update(uint percentage, string status);
        bool Visible { get; set; }
    }
}
