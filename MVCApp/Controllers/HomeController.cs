using Infrastructure.Domain;
using Infrastructure.Mapping;
using Infrastructure.UnitOfWork;
using Model.Customers;
using Repository;
using Repository.Mapping.SQL;
using Repository.UnitOfWork;
using Respository.Mapping.SQL.Base;
using Session;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCApp.Controllers
{
    //NOTE: Exception handlers could be better addressed by implementing the Messages Pattern. For another time!
    public class HomeController : Controller
    {
        private IUnitOfWork _uow;
        private IDataMapper _mapper;
        private CustomerRepository _repository;
        private Customer _customer;

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Test()
        {
            ViewBag.ReadLock = true;
            ViewBag.Released = false;

            PreparePlayers();
            ViewBag.CustomerName = _customer != null ? _customer.FullName + " (READ)" : "No DATA";
            ViewBag.Message = _customer != null ? "Session has READ Lock" : "Couldn't get READ Lock";
            return View();
        }

        public ActionResult UpdateCustomer()
        {
            ViewBag.Released = false;
            ViewBag.Message = "Session got WRITE Lock and SAVED customer data, now has READ Lock";

            PreparePlayers();
            var connection = SessionFactory.GetCurrentSession().DbInfo.Connection;
            connection.Open();
            try
            {
                _customer.FirstName = "Kaike_" + new Random().Next(1000).ToString();
                _repository.Update(_customer);
                _uow.Commit();
            }
            catch (Exception e)
            {
                ViewBag.Message = string.Format("Exception ocurred, new name COULD NOT be saved. {0}", e.Message);
            }
            finally
            {
                connection.Close();
            }
            ViewBag.CustomerName = _customer.FullName + " (EDITED)";

            ViewBag.ReadLock = true;
            return View("Test");
        }

        public ActionResult ReleaseCustomer()
        {
            ViewBag.ReadLock = false;
            ViewBag.Released = true;
            ViewBag.Message = "Session has REALEASED Customer";

            var connection = SessionFactory.GetCurrentSession().DbInfo.Connection;
            connection.Open();
            try
            {
                SessionFactory.GetCurrentSession().LockManager.ReleaseLock(new Guid("da365eb6-74c2-4d60-aa33-ac1af3637b1a"));
                CleanupPlayers();
            }
            catch (Exception e)
            {
                ViewBag.Message = string.Format("Exception ocurred. {0}", e.Message);
            }
            finally
            {
                connection.Close();
            }
            ViewBag.CustomerName = "No Customer selected!";

            return View("Test");
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        private void PreparePlayers()
        {
            _uow = new UnitOfWork();
            _mapper = new LockingMapper(new CustomerSQLMapper());
            _repository = new CustomerRepository(_uow, _mapper);
            _customer = _repository.FindBy(new Guid("da365eb6-74c2-4d60-aa33-ac1af3637b1a"));
        }

        private void CleanupPlayers()
        {
            _uow = null;
            _mapper = null;
            _repository = null;
            _customer = null;
        }
    }
}