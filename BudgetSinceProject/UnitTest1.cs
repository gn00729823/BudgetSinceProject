using NUnit.Framework;
using System;
using System.Collections.Generic;
namespace BudgetSinceProject
{
    public class Tests
    {
        private BudgetService _budgetService;
        private FakeBudgetRepo _budgetRepo;
        [SetUp]
        public void Setup()
        {
            _budgetRepo = new FakeBudgetRepo();
            _budgetService = new BudgetService(_budgetRepo);
        }

        [Test]
        public void start_bigger_Then_end()
        {
            var budget = _budgetService.Query(GetDateTime(2022, 2, 2), GetDateTime(2022, 2, 1));
            Assert.AreEqual(0, budget);
        }

        [Test]
        public void get_a_month()
        {
            var fakeData = new List<Budget>();
            fakeData.Add(new Budget() { amount = 310, yearMonth = "202201" });
            _budgetRepo.AllBudget = fakeData;


            var budget = _budgetService.Query(GetDateTime(2022, 1, 1), GetDateTime(2022, 1, 31));
            Assert.AreEqual(310, budget);
        }

        [Test]
        public void get_a_day()
        {
            var fakeData = new List<Budget>();
            fakeData.Add(new Budget() { amount = 310, yearMonth = "202201" });
            _budgetRepo.AllBudget = fakeData;


            var budget = _budgetService.Query(GetDateTime(2022, 1, 1), GetDateTime(2022, 1, 1));
            Assert.AreEqual(10, budget);
        }

        [Test]
        public void get_cross_month()
        {
            var fakeData = new List<Budget>();
            fakeData.Add(new Budget() { amount = 310, yearMonth = "202201" });
            fakeData.Add(new Budget() { amount = 28, yearMonth = "202202" });
            _budgetRepo.AllBudget = fakeData;


            var budget = _budgetService.Query(GetDateTime(2022, 1, 31), GetDateTime(2022, 2, 2));
            Assert.AreEqual(10 + 2, budget);
        }

        [Test]
        public void get_cross_three_month()
        {
            var fakeData = new List<Budget>();
            fakeData.Add(new Budget() { amount = 310, yearMonth = "202201" });
            fakeData.Add(new Budget() { amount = 28, yearMonth = "202202" });
            fakeData.Add(new Budget() { amount = 31, yearMonth = "202203" });
            _budgetRepo.AllBudget = fakeData;


            var budget = _budgetService.Query(GetDateTime(2022, 1, 31), GetDateTime(2022, 3, 2));
            Assert.AreEqual(10 + 28 + 2, budget);
        }

        [Test]
        public void get_a_year()
        {
            var fakeData = new List<Budget>();
            fakeData.Add(new Budget() { amount = 310, yearMonth = "202201" });
            fakeData.Add(new Budget() { amount = 28, yearMonth = "202202" });
            fakeData.Add(new Budget() { amount = 31, yearMonth = "202203" });
            _budgetRepo.AllBudget = fakeData;


            var budget = _budgetService.Query(GetDateTime(2022, 1, 1), GetDateTime(2023, 1, 1));
            Assert.AreEqual(310 + 28 + 31, budget);
        }

        [Test]
        public void get_cross_year()
        {
            var fakeData = new List<Budget>();
            fakeData.Add(new Budget() { amount = 310, yearMonth = "202212" });
            fakeData.Add(new Budget() { amount = 31, yearMonth = "202301" });
            _budgetRepo.AllBudget = fakeData;


            var budget = _budgetService.Query(GetDateTime(2022, 12, 31), GetDateTime(2023, 1, 31));
            Assert.AreEqual(10 + 31, budget);
        }

        private DateTime GetDateTime(int year, int month, int day)
        {

            return new DateTime(year, month, day);


        }


        private class FakeBudgetRepo : IBudgetRepo
        {
            private List<Budget> allBudget = new List<Budget>();

            public List<Budget> AllBudget
            {
                set => allBudget = value;
            }

            public List<Budget> GetAll()
            {
                return allBudget;
            }

        }
    }

    public class BudgetService
    {
        private IBudgetRepo _budgetRepo;

        public BudgetService(IBudgetRepo budgetRepo)
        {
            _budgetRepo = budgetRepo;
        }


        public double Query(DateTime start, DateTime end)
        {

            if (start > end)
                return 0;

            double total = 0;
            DateTime curDateTime = start;

            while (curDateTime < new DateTime(end.Year, end.Month, 1).AddMonths(1))
            {
                int diffDay = 0;
                if (curDateTime.Year == start.Year && curDateTime.Month == start.Month)
                {
                    if (start.Year == end.Year && start.Month == end.Month)
                    {
                        diffDay = end.Day - start.Day + 1;
                    }
                    else
                    {
                        diffDay = DateTime.DaysInMonth(start.Year, start.Month) - start.Day + 1;
                    }
                }
                else if (curDateTime.Year == end.Year && curDateTime.Month == end.Month)
                {
                    diffDay = end.Day;
                }
                else
                {
                    diffDay = DateTime.DaysInMonth(curDateTime.Year, curDateTime.Month);
                }



                total += GetBudget(curDateTime, diffDay);





                curDateTime = curDateTime.AddMonths(1);

            }


            return total;
        }

        private double GetBudget(DateTime date, int day)
        {
            var startDaysInMonth = GetDaysInMonth(date);
            var budgetList = _budgetRepo.GetAll();
            var monthBudget = budgetList.Find(x => x.yearMonth == date.ToString("yyyyMM"));
            if (monthBudget == null)
            {
                return 0;
            }
            return (monthBudget.amount / startDaysInMonth) * day;
        }

        private static int GetDaysInMonth(DateTime dataTime)
        {
            return DateTime.DaysInMonth(dataTime.Year, dataTime.Month);
        }
    }



    public interface IBudgetRepo
    {
        List<Budget> GetAll();
    }

    public class Budget
    {
        public string yearMonth;
        public int amount;
    }
}