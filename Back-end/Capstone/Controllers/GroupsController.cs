﻿using AutoMapper;
using Capstone.Model;
using Capstone.Service;
using Capstone.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Capstone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IGroupService _groupService;

        public GroupsController(IMapper mapper, IGroupService groupService)
        {
            _mapper = mapper;
            _groupService = groupService;
        }

        // POST: api/Groups
        [HttpPost]
        public ActionResult<Group> PostGroup(GroupCM model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                Group group = new Group();
                group = _mapper.Map<Group>(model);
                _groupService.Create(group);
                _groupService.Save();
                return StatusCode(201, "Group Created!");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // GET: api/Groups
        [HttpGet]
        public ActionResult<IEnumerable<Group>> GetGroups()
        {
            try
            {
                List<GroupVM> result = new List<GroupVM>();
                var data = _groupService.GetAll();
                foreach (var item in data)
                {
                    result.Add(_mapper.Map<GroupVM>(item));
                }
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // GET: api/Groups/5
        [HttpGet("GetByID")]
        public ActionResult<Group> GetGroup(Guid ID)
        {
            try
            {
                var rs = _groupService.GetByID(ID);
                if (rs == null) return NotFound("ID not found!");
                GroupVM result = _mapper.Map<GroupVM>(rs);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        
        // PUT: api/Groups/5
        [HttpPut]
        public IActionResult PutGroup(GroupUM model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var groupInDb = _groupService.GetByID(model.ID);
                if (groupInDb == null) return NotFound("ID not found!");
                _mapper.Map(model, groupInDb);
                _groupService.Save();
                return Ok("success");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // DELETE: api/Groups/5
        [HttpDelete]
        public ActionResult DeleteGroup(Guid ID)
        {
            try
            {
                var groupInDb = _groupService.GetByID(ID);
                if (groupInDb == null) return NotFound("ID not found!");
                groupInDb.IsDelete = true;
                _groupService.Save();
                return Ok("success");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
