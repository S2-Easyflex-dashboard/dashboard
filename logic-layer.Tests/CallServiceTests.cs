using Microsoft.VisualStudio.TestTools.UnitTesting;
using logic_layer;
using System;

namespace logic_layer.Tests
{

    [TestClass]
    public class CallServiceTests
    {


        [DataTestMethod]
        [DataRow("10.0.0.1", 5, 0)]        // internal IP (starts with "10.")
        [DataRow("192.168.1.1", 0, 5)]     // external IP
        public void GetInternVsExtern_OneCall_AddsAmountToCorrectIndex(
    string ip, int expectedInternal, int expectedExternal)
        {
            // Arrange
            var call = new CallModel(
                customerId: 1,
                date: DateOnly.Parse("2026-06-06"),
                ip: ip,
                service: "someService",
                amount: 5,
                licentionNr: 100
            );

            var calls = new List<CallModel> { call };

            // Act
            int[] result = CallService.GetInternVsExtern(null, null, calls);

            // Assert
            Assert.AreEqual(expectedInternal, result[0]);
            Assert.AreEqual(expectedExternal, result[1]);
        }


        [TestMethod]
        public void GetInternVsExtern_EmptyList_ReturnsZeros()
        {
            // Arrange — empty list of calls
            var calls = new List<CallModel>();

            // Act
            int[] result = CallService.GetInternVsExtern(null, null, calls);

            // Assert — both counters should be 0
            Assert.AreEqual(0, result[0]);  // internal = 0
            Assert.AreEqual(0, result[1]);  // external = 0
        }

        [TestMethod]
        public void GetInternVsExtern_MultipleCalls_SumsAmountsCorrectly()
        {
            // Arrange — list with 2 internal and 1 external calls
            var call1 = new CallModel(
                customerId: 1,
                date: DateOnly.Parse("2026-06-06"),
                ip: "10.0.0.1",          // internal
                service: "someService",
                amount: 5,
                licentionNr: 100
            );

            var call2 = new CallModel(
                customerId: 2,
                date: DateOnly.Parse("2026-06-06"),
                ip: "192.168.1.1",       // external
                service: "someService",
                amount: 3,
                licentionNr: 101
            );

            var call3 = new CallModel(
                customerId: 3,
                date: DateOnly.Parse("2026-06-06"),
                ip: "10.5.5.5",          // internal
                service: "someService",
                amount: 2,
                licentionNr: 102
            );

            var calls = new List<CallModel> { call1, call2, call3 };

            // Act
            int[] result = CallService.GetInternVsExtern(null, null, calls);

            // Assert — internal = 5 + 2 = 7, external = 3
            Assert.AreEqual(7, result[0]);  // internal = 7
            Assert.AreEqual(3, result[1]);  // external = 3
        }



        [DataTestMethod]
        [DataRow("abc_fw_xyz", 5, 0, 0)]   // _fw_ → flex
        [DataRow("abc_rl_xyz", 0, 5, 0)]   // _rl_ → relation
        [DataRow("abc_wm_xyz", 0, 0, 5)]   // _wm_ → managing
        [DataRow("abc_bi_xyz", 0, 0, 5)]   // _bi_ → managing
        public void SplitCallsPerService_BasicServices_AddsToCorrectLevel(
    string serviceName, int expectedFlex, int expectedRelation, int expectedManaging)
        {
            // Arrange
            var call = new CallModel(
                customerId: 1,
                date: DateOnly.Parse("2026-06-06"),
                ip: "10.0.0.1",
                service: serviceName,
                amount: 5,
                licentionNr: 100
            );

            var calls = new List<CallModel> { call };

            // Act
            int[] result = CallService.SplitCallsPerService(false, false, calls);

            // Assert
            Assert.AreEqual(expectedFlex, result[0]);
            Assert.AreEqual(expectedRelation, result[1]);
            Assert.AreEqual(expectedManaging, result[2]);
        }





        [DataTestMethod]
        [DataRow(false, false, 0, 0, 0)]   // no filters → call is ignored
        [DataRow(true, false, 5, 0, 0)]    // TempHireFilter → adds to flex
        [DataRow(false, true, 0, 5, 0)]    // RelationFilter → adds to relation
        [DataRow(true, true, 5, 5, 0)]     // both filters → adds to BOTH flex and relation (double count)
        public void SplitCallsPerService_RfService_WithVariousFilters(
     bool tempHireFilter, bool relationFilter,
     int expectedFlex, int expectedRelation, int expectedManaging)
        {
            // Arrange — one call with "_rf_" in service
            var call = new CallModel(
                customerId: 1,
                date: DateOnly.Parse("2026-06-06"),
                ip: "10.0.0.1",
                service: "abc_rf_xyz",
                amount: 5,
                licentionNr: 100
            );

            var calls = new List<CallModel> { call };

            // Act
            int[] result = CallService.SplitCallsPerService(tempHireFilter, relationFilter, calls);

            // Assert
            Assert.AreEqual(expectedFlex, result[0]);
            Assert.AreEqual(expectedRelation, result[1]);
            Assert.AreEqual(expectedManaging, result[2]);
        }


        [TestMethod]
        public void GetAverageCallsPerDay_EmptyList_ThrowsDivideByZeroException()
        {
            // Bug: the method crashes on an empty list because of division by zero.

            // Arrange
            var calls = new List<CallModel>();

            // Act + Assert
            Assert.ThrowsExactly<DivideByZeroException>(() =>
                CallService.GetAverageCallsPerDay(null, null, calls));
        }


        [TestMethod]
        public void GetAverageCallsPerDay_SomeDaysMissing_ThrowsDivideByZeroException()
        {
            // Bug: the method crashes if any day of the week has no calls (division by zero).

            // Arrange — only Monday and Tuesday have calls, other days are missing
            var calls = new List<CallModel>
    {
        new CallModel(1, DateOnly.Parse("2024-01-08"), "10.0.0.1", "service", 10, 100), // Monday
        new CallModel(1, DateOnly.Parse("2024-01-09"), "10.0.0.1", "service", 10, 100), // Tuesday
    };

            // Act + Assert
            Assert.ThrowsExactly<DivideByZeroException>(() =>
                CallService.GetAverageCallsPerDay(null, null, calls));
        }


        [TestMethod]
        public void GetAverageCallsPerDay_WithCustomerFilter_OnlyIncludesMatchingCustomer()
        {
            // Arrange — calls from two customers on the same days
            // Customer 1 has Amount 100 on each day, customer 2 has Amount 999 (should be ignored)
            var calls = new List<CallModel>
    {
        // Customer 1 — all 7 days
        new CallModel(1, DateOnly.Parse("2024-01-07"), "10.0.0.1", "service", 100, 100), // Sunday
        new CallModel(1, DateOnly.Parse("2024-01-08"), "10.0.0.1", "service", 100, 100), // Monday
        new CallModel(1, DateOnly.Parse("2024-01-09"), "10.0.0.1", "service", 100, 100), // Tuesday
        new CallModel(1, DateOnly.Parse("2024-01-10"), "10.0.0.1", "service", 100, 100), // Wednesday
        new CallModel(1, DateOnly.Parse("2024-01-11"), "10.0.0.1", "service", 100, 100), // Thursday
        new CallModel(1, DateOnly.Parse("2024-01-12"), "10.0.0.1", "service", 100, 100), // Friday
        new CallModel(1, DateOnly.Parse("2024-01-13"), "10.0.0.1", "service", 100, 100), // Saturday

        // Customer 2 — should be filtered out
        new CallModel(2, DateOnly.Parse("2024-01-08"), "10.0.0.1", "service", 999, 100), // Monday
        new CallModel(2, DateOnly.Parse("2024-01-09"), "10.0.0.1", "service", 999, 100), // Tuesday
    };

            // Act — filter by customerId = 1
            int[] result = CallService.GetAverageCallsPerDay(1, null, calls);

            // Assert — only customer 1's calls should be counted, average = 100 for each day
            Assert.AreEqual(100, result[0]);  // Sunday
            Assert.AreEqual(100, result[1]);  // Monday
            Assert.AreEqual(100, result[2]);  // Tuesday
            Assert.AreEqual(100, result[3]);  // Wednesday
            Assert.AreEqual(100, result[4]);  // Thursday
            Assert.AreEqual(100, result[5]);  // Friday
            Assert.AreEqual(100, result[6]);  // Saturday
        }


        [TestMethod]
        public void GetAlerts_CustomerWithMoreThan1440InOneDay_ReturnsProbleem()
        {
            // Arrange — one customer with 1500 calls on a single day (> 1440 → Probleem)
            var calls = new List<CallModel>
    {
        new CallModel(1, DateOnly.Parse("2024-01-08"), "10.0.0.1", "service", 1500, 100),
    };

            // Act
            List<AlertModel> result = CallService.GetAlerts(calls);

            // Assert
            Assert.AreEqual(1, result.Count);              // exactly one alert
            Assert.AreEqual(1, result[0].CustomerId);      // for customer 1
            Assert.AreEqual("Probleem", result[0].AlertType);
        }





        [DataTestMethod]
        [DataRow(5, "Waarschuwing")]   // 5 consecutive days above 120 → Waarschuwing
        [DataRow(10, "Probleem")]      // 10 consecutive days above 120 → Probleem
        public void GetAlerts_ConsecutiveDaysAbove120_ReturnsCorrectAlertType(int daysCount, string expectedType)
        {
            // Arrange — create N consecutive days starting from 2024-01-01, each with Amount = 150 (> 120)
            var calls = new List<CallModel>();
            var startDate = DateOnly.Parse("2024-01-01");

            for (int i = 0; i < daysCount; i++)
            {
                calls.Add(new CallModel(1, startDate.AddDays(i), "10.0.0.1", "service", 150, 100));
            }

            // Act
            List<AlertModel> result = CallService.GetAlerts(calls);

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1, result[0].CustomerId);
            Assert.AreEqual(expectedType, result[0].AlertType);
        }



        [TestMethod]
        public void GetAlerts_CustomerWithSingleDayAbove120_ReturnsWaarschuwing()
        {
            // Arrange — one day with Amount = 150 (> 120 but not > 1440, no consecutive sequence)
            var calls = new List<CallModel>
    {
        new CallModel(1, DateOnly.Parse("2024-01-08"), "10.0.0.1", "service", 150, 100),
    };

            // Act
            List<AlertModel> result = CallService.GetAlerts(calls);

            // Assert — single day > 120 should trigger Waarschuwing
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1, result[0].CustomerId);
            Assert.AreEqual("Waarschuwing", result[0].AlertType);
        }



        [TestMethod]
        public void GetAlerts_CustomerWithExactly1440_ReturnsWaarschuwing()
        {
            // Arrange — Amount = exactly 1440 (boundary). NOT > 1440 → not Probleem.
            // But > 120, so Waarschuwing is triggered.
            var calls = new List<CallModel>
    {
        new CallModel(1, DateOnly.Parse("2024-01-08"), "10.0.0.1", "service", 1440, 100),
    };

            // Act
            List<AlertModel> result = CallService.GetAlerts(calls);

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1, result[0].CustomerId);
            Assert.AreEqual("Waarschuwing", result[0].AlertType);
        }


        [TestMethod]
        public void GetAlerts_EmptyList_ReturnsEmptyList()
        {
            // Arrange — no calls
            var calls = new List<CallModel>();

            // Act
            List<AlertModel> result = CallService.GetAlerts(calls);

            // Assert — no customers, no alerts
            Assert.AreEqual(0, result.Count);
        }



    }

}
