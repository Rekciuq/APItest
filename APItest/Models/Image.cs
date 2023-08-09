﻿namespace APItest.Models
{
    public class Image
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public byte[]? ImageInBytes { get; set; } = null;
    }
}
