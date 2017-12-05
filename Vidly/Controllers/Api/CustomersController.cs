﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using Vidly.DTOs;
using Vidly.Models;

namespace Vidly.Controllers.Api
{
    
    public class CustomersController : ApiController
    {
        #region ApplicationDbContext
        private ApplicationDbContext _context;

        public CustomersController()
        {
            _context = new ApplicationDbContext();
        }
        #endregion

        //In APIs we should not use domain objects (e.g. Customer) because it introduces security vulnerabilities
        //and it reduces the chances of our API breaking if we change our domain model.
        //We should use Data Transfer Objects (DTO) instead

        // GET /api/customers
        public IEnumerable<CustomerDTO> GetCustomers()
        {
            return _context.Customers.ToList().Select(Mapper.Map<Customer, CustomerDTO>); //Map<...> without () in the end - because we want deferred exec
        }

        // GET /api/customers/1
        public IHttpActionResult GetCustomer(int id)
        {
            var customer = _context.Customers.SingleOrDefault(x => x.Id == id);

            if (customer == null)
                return NotFound();

            return Ok(Mapper.Map<Customer, CustomerDTO>(customer));
            //return customer;
        }

        // POST /api/customers
        [HttpPost] //only responds to POST requests
        //IHttpActionResult similar to actionResult in MVC. 
        public IHttpActionResult CreateCustomer(CustomerDTO customerDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var customer = Mapper.Map<CustomerDTO, Customer>(customerDTO);

            _context.Customers.Add(customer);
            _context.SaveChanges();

            customerDTO.Id = customer.Id;

            return Created(new Uri(Request.RequestUri + "/" + customer.Id), customerDTO);
        }

        // PUT /api/customers/1
        [HttpPut] //only responds to PUT requests
        public void UpdateCustomer(int id, CustomerDTO customerDTO)
        {
            if (!ModelState.IsValid)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            var customerInDb = _context.Customers.SingleOrDefault(x => x.Id == id);

            if (customerInDb == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            //existing obj passed as 2nd arg to track changes to it
            Mapper.Map<CustomerDTO, Customer>(customerDTO, customerInDb);

            //customerInDb.Name = customerDTO.Name;
            //customerInDb.Birthdate = customerDTO.Birthdate;
            //customerInDb.IsSubscribedToNewsletter = customerDTO.IsSubscribedToNewsletter;
            //customerInDb.MembershipTypeId = customerDTO.MembershipTypeId;

            _context.SaveChanges();
        }

        //DELETE api/customers/1
        [HttpDelete]
        public void DeleteCustomer(int id)
        {
            var customerInDb = _context.Customers.SingleOrDefault(x => x.Id == id);

            if (customerInDb == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            _context.Customers.Remove(customerInDb);
            _context.SaveChanges();
        }
    }
}
