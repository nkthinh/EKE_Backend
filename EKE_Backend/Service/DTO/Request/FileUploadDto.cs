using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO.Request
{
    public class FileUploadDto
    {
        [Required(ErrorMessage = "Image file is required")]
        public IFormFile ImageFile { get; set; } = null!;
    }
}
