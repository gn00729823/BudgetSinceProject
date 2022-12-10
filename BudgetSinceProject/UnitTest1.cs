using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices.ComTypes;
using NUnit.Framework;
using NSubstitute;
namespace BudgetSinceProject
{
    public class Tests
    {
        private BudgetService _budgetService;
        private IbudgetRepo budgetRepo;
        [SetUp]
        public void Setup()
        {
            budgetRepo = new TestBudgetRepo();
            _budgetService = new BudgetService(budgetRepo);
        }

        [Test]
        public void start_bigger_Then_end()
        {
         var budget=   _budgetService.Query(DateTime.Today, DateTime.MinValue);
           Assert.AreEqual(0,budget);
        }

        private class TestBudgetRepo : IbudgetRepo
        {
            public List<Budget> GetAll()
            {
                throw new NotImplementedException();
            }
        }
    }

    public class BudgetService
    {
        private IbudgetRepo _budgetRepo;

        public BudgetService(IbudgetRepo budgetRepo)
        {

        }


        public double Query(DateTime start, DateTime end)
        {
            var budgetList = _budgetRepo.GetAll();
            if (start > end)
                return 0;

            return 0;
        }
    }



    public interface IbudgetRepo
    {
        List<Budget> GetAll();
    }

    public class Budget
    {
        public string yearMonth;
        public int amount;
    }
}