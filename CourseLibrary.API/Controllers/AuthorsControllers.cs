﻿using AutoMapper;
using CourseLibrary.API.Helpers;
using CourseLibrary.API.Model;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.Controllers
{
    // The basics that should be done while implementing the outer-facing contract for the API
    // 1. Controller
    // 2. Inject repository
    // 3. Create action methods
    // 4. Return results.
    [ApiController]
    [Route("api/authors")]
    public class AuthorsControllers : ControllerBase
    {
        private readonly ICourseLibraryRepository _courseLibraryRepository;
        private readonly IMapper _mapper;

        public AuthorsControllers(ICourseLibraryRepository courseLibraryRepository, IMapper mapper)
        {
            _courseLibraryRepository = courseLibraryRepository ?? throw new ArgumentNullException(nameof(courseLibraryRepository));

            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet()]
        [HttpHead] // Same as GET but returns an object without the response body. Typically used to get information about a resource. 
        public ActionResult<IEnumerable<AuthorDto>> GetAuthors([FromQuery] string mainCategory)
        {
            var authorsFromRepo = _courseLibraryRepository.GetAuthors(mainCategory);                    

            return Ok(_mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepo));
        }

        [HttpGet("{authorId}")]
        public IActionResult GetAuthor(Guid authorId)
        {
            var authorFromRepo = _courseLibraryRepository.GetAuthor(authorId);

            if (authorFromRepo == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<AuthorDto>(authorFromRepo));
        }
    }
}
