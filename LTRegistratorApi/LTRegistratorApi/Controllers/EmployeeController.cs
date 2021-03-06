﻿using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using LTRegistrator.BLL.Contracts;
using LTRegistrator.BLL.Contracts.Contracts;
using LTRegistrator.Domain.Entities;
using LTRegistratorApi.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace LTRegistratorApi.Controllers
{
    /// <summary>
    /// Controller providing basic employee operations
    /// </summary>
    [Route("api/[controller]")]
    [ApiController, Authorize]
    public class EmployeeController : BaseController
    {
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="employeeService"></param>
        /// <param name="mapper"></param>
        /// <param name="db"></param>
        public EmployeeController(IEmployeeService employeeService, IMapper mapper, DbContext db) : base(db)
        {
            _employeeService = employeeService;
            _mapper = mapper;
        }

        /// <summary>
        /// Sends this user information.
        /// </summary>
        /// <param name="employeeId">UserId</param>
        /// <returns> Basic Employee information </returns>
        /// <response code="200">Basic Employee information</response>
        /// <response code="404">User not found </response>
        [HttpGet("{employeeId}/info")]
        [ProducesResponseType(typeof(EmployeeDto), 200)]
        [ProducesResponseType(404)]
        [Authorize(Policy = "AccessAllowed")]
        public async Task<ActionResult> GetInfoAsync(int employeeId)
        {
            var response = await _employeeService.GetByIdAsync(employeeId);

            return response.Status == ResponseResult.Success
            ? Ok(_mapper.Map<EmployeeDto>(response.Result))
            : StatusCode((int)response.Error.StatusCode, response.Error.Message);
        }

        /// <summary>
        /// Gets a list of all human leaves.
        /// </summary>
        /// <param name="employeeId">UserId</param>
        /// <returns>User's leave list</returns>
        /// <response code="200">User's leave list</response>
        /// <response code="404">User not found </response>
        [HttpGet("{employeeId}/leaves")]
        [ProducesResponseType(typeof(ICollection<LeaveDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> GetLeavesAsync(int employeeId)
        {
            var response = await _employeeService.GetByIdAsync(employeeId);

            return response.Status == ResponseResult.Success
                ? Ok(_mapper.Map<ICollection<LeaveDto>>(response.Result.Leaves))
                : StatusCode((int)response.Error.StatusCode, response.Error.Message);
        }

        /// <summary>
        /// POST api/employee/{id}/leaves
        /// Add new leaves for user.
        /// </summary>
        /// <param name="employeeId">UserId</param>
        /// <param name="leaves">List of LeaveDto that is added to the user</param>
        /// <returns>Was the operation successful?</returns>
        /// <response code="200">Operation successful</response>
        /// <response code="400">Bad request</response>
        /// <response code="404">Employee not found</response>
        [HttpPost("{employeeId}/leaves")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> SetLeavesAsync(int employeeId, [FromBody] ICollection<LeaveInputDto> leaves)
        {
            if (leaves == null)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _employeeService.AddLeavesAsync(employeeId, _mapper.Map<ICollection<Leave>>(leaves));
            return response.Status == ResponseResult.Success ? (ActionResult)Ok() : StatusCode((int)response.Error.StatusCode, new { response.Error.Message });
        }

        /// <summary>
        /// PUT api/employee/{id}/leaves
        /// Updates information on leaves.
        /// </summary>
        /// <param name="employeeId">UserId</param>
        /// <param name="leaves">List of leaves that updated</param>
        /// <returns>Was the operation successful?</returns>
        /// <response code="200">Operation successful</response>
        /// <response code="400">Bad request</response>
        /// <response code="404">Employee not found</response>
        [HttpPut("{employeeId}/leaves")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> UpdateLeavesAsync(int employeeId, [FromBody] List<LeaveDto> leaves)
        {
            if (leaves == null)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _employeeService.UpdateLeavesAsync(employeeId, _mapper.Map<ICollection<Leave>>(leaves));
            return response.Status == ResponseResult.Success ? (ActionResult)Ok() : StatusCode((int)response.Error.StatusCode, response.Error.Message);
        }

        /// <summary>
        /// DELETE api/employee/{id}/leaves?leaveID=1&leaveID=2&leaveID=3
        /// Deletes a leaves record.
        /// </summary>
        /// <param name="employeeId">UserId</param>
        /// <param name="leaveID"> IDs of leaves that should be deleted</param>
        /// <returns>Was the operation successful?</returns>
        /// <response code="200">Operation successful</response>
        /// <response code="400">Bad request</response>
        /// <response code="403">You cannot change another employee’s leave</response>
        /// <response code="404">Employee not found</response>
        [HttpDelete("{employeeId}/leaves")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> DeleteLeavesAsync(int employeeId, [FromQuery] List<int> leaveID)
        {
            if (leaveID == null)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _employeeService.DeleteLeavesAsync(employeeId, leaveID);
            return response.Status == ResponseResult.Success ? (ActionResult)Ok() : StatusCode((int)response.Error.StatusCode, response.Error.Message);
        }
    }
}
