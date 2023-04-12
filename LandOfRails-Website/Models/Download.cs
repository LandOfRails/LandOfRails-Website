using System;
using System.Collections.Generic;

namespace LandOfRails_Website.Models
{
    public partial class Download
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string DownloadLink { get; set; }
        public string BackgroundImageLink { get; set; }
    }
}
