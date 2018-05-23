using LagoVista.Core.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace LagoVista.IoT.DeviceManagement.Core.Tests
{
    public class ValidationBase
    {
        protected void AssertInvalidError(InvokeResult result, params string[] errs)
        {
            Console.WriteLine("Errors (at least some are expected)");

            foreach (var err in result.Errors)
            {
                Console.WriteLine(err.Message);
            }

            foreach (var err in errs)
            {
                Assert.IsTrue(result.Errors.Where(msg => msg.Message == err).Any(), $"Could not find error [{err}]");
            }

            Assert.AreEqual(errs.Length, result.Errors.Count, "Validation error mismatch between");

            Assert.IsFalse(result.Successful, "Validated as successful but should have failed.");
        }

        protected void AssertInvalidError(ValidationResult result, params string[] errs)
        {
            Console.WriteLine("Errors (at least some are expected)");

            foreach (var err in result.Errors)
            {
                Console.WriteLine(err.Message);
            }

            foreach (var err in errs)
            {
                Assert.IsTrue(result.Errors.Where(msg => msg.Message == err).Any(), $"Could not find error [{err}]");
            }

            Assert.AreEqual(errs.Length, result.Errors.Count, "Validation error mismatch between");

            Assert.IsFalse(result.Successful, "Validated as successful but should have failed.");
        }

        protected void AssertSuccessful(InvokeResult result)
        {
            if (result.Errors.Any())
            {
                Console.WriteLine("unexpected errors");
            }

            foreach (var err in result.Errors)
            {
                Console.WriteLine("\t" + err.Message);
            }

            Assert.IsTrue(result.Successful);
        }

        protected void AssertSuccessful(ValidationResult result)
        {
            if (result.Errors.Any())
            {
                Console.WriteLine("unexpected errors");
            }

            foreach (var err in result.Errors)
            {
                Console.WriteLine("\t" + err.Message);
            }

            Assert.IsTrue(result.Successful);
        }

    }
}
