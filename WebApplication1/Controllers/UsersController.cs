﻿using Core.Entities.Attendance;
using Core.Entities.Login;
using Core.Repositories;
using Core.Repositories.DTOs;
using Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Controllers
{
    [Authorize(Policy = "User")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        private readonly IUsersServices userService;
        private readonly IUsersRepository userRepository;
        public UsersController(IUsersServices userService, IUsersRepository userRepository)
        {
            this.userService = userService;
            this.userRepository = userRepository;
        }

        // get/users/ attendanceName/ userName
        [HttpGet("attendance/{attendanceName}/{userName}")]
        [ProducesResponseType(201, Type = typeof(Attendance))]
        public async Task<IActionResult> GetAttendanceByUser(string attendanceName, string userName )
        {
            var newAttendance = await userRepository.getAttendance(attendanceName, userName);
            if (newAttendance == null)
                return NotFound();
            return Ok(newAttendance);
        }

        //  put/users
        [HttpPut]
        [ProducesResponseType(201, Type = typeof(Attendance))]
        public async Task<IActionResult> UpdateAttendance([FromBody] Attendance attendance)
        {
            var newAttendance = await userService.PutAttendance(attendance);
            if (newAttendance == null)
                return NotFound();
            return Ok(attendance);
        }

        // get/users/timetable
        [HttpGet("timetable")]
        [ProducesResponseType(201, Type = typeof(TimetableDTO))]
        public async Task<IActionResult> GetAllTimetables()
        {
            var timetable = await userRepository.GetAllTimetables();
            if (timetable == null)
            {
                return BadRequest();
            }

            return Ok(timetable);
        }

        // get/users/timetable/Cours name
        [HttpGet("timetable/{name}")]
        [ProducesResponseType(201, Type = typeof(TimetableDTO))]
        public async Task<IActionResult> GetTimetables(string name)
        {
            var timetable = await userRepository.GetAllTimetables();
            if (timetable == null)
            {
                return BadRequest();
            }

            return Ok(timetable);
        }


        // get users ionut
        [HttpGet("{username}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUserName(string username)
        {
            var user = await userService.GetUserAsync(username);
            return Ok(user);

        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateNewUserAccountAsync([FromBody] LoginDTO newUser)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                User user = await userService.RegisterNewUserAsync(newUser);
                if (user == null)
                {
                    return BadRequest(user);
                }
                else
                {
                    var uri = new Uri($"{Request.GetDisplayUrl()}/{user.Id}");

                    return Created(uri, user);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }

        }
    }
}


