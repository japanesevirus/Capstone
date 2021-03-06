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
    public class RolesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IRoleService _roleService;

        public RolesController(IMapper mapper, IRoleService roleService)
        {
            _mapper = mapper;
            _roleService = roleService;
        }

        // POST: api/Roles
        [HttpPost]
        public ActionResult<Role> PostRole(RoleCM model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                Role role = new Role();
                role = _mapper.Map<Role>(model);
                _roleService.Create(role);
                _roleService.Save();
                return StatusCode(201, "Role Type Created!");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // GET: api/Roles
        [HttpGet]
        public ActionResult<IEnumerable<Role>> GetRoles()
        {
            try
            {
                List<RoleVM> result = new List<RoleVM>();
                var data = _roleService.GetAll();
                foreach (var item in data)
                {
                    result.Add(_mapper.Map<RoleVM>(item));
                }
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // GET: api/Roles/GetByID
        [HttpGet("GetByID")]
        public ActionResult<Role> GetRole(Guid ID)
        {
            try
            {
                var rs = _roleService.GetByID(ID);
                if (rs == null) return BadRequest("ID not found!");
                RoleVM result = _mapper.Map<RoleVM>(rs);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // PUT: api/Roles
        [HttpPut]
        public IActionResult PutRole(RoleUM model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var roleInDb = _roleService.GetByID(model.ID);
                if (roleInDb == null) return BadRequest("ID not found!");
                _mapper.Map(model, roleInDb);
                _roleService.Save();
                return Ok("success");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // DELETE: api/Roles
        [HttpDelete]
        public ActionResult DeleteRole(Guid ID)
        {
            try
            {
                var roleInDb = _roleService.GetByID(ID);
                if (roleInDb == null) return BadRequest("ID not found!");
                roleInDb.IsDelete = true;
                _roleService.Save();
                return Ok("success");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}