using System.Collections.Generic;
using System.Linq;
using Glasswall.IdentityManagementService.Api.ActionFilters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using TestCommon;

namespace Service.Tests.ModelStateValidationActionFilterAttributeTests
{
    [TestFixture]
    public class ModelStateValidationActionFilterAttributeTests : UnitTestBase<ModelStateValidationActionFilterAttribute>
    {
        [Test]
        public void Errors_Return_BadRequest()
        {
            ClassInTest = new ModelStateValidationActionFilterAttribute();

            var httpContextMock = new DefaultHttpContext
            {
                RequestServices = new ServiceCollection().AddLogging().BuildServiceProvider()
            };

            var modelState = new ModelStateDictionary();
            modelState.AddModelError("name", "invalid");

            var actionContext = new ActionContext(
                httpContextMock,
                Mock.Of<RouteData>(),
                Mock.Of<ActionDescriptor>(),
                modelState
            );

            var actionExecutingContext = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                Mock.Of<Controller>()
            );

            ClassInTest.OnActionExecuting(actionExecutingContext);

            Assert.That(actionExecutingContext.Result, Is.InstanceOf<BadRequestObjectResult>().With.Property(nameof(BadRequestObjectResult.Value))
                .With.Property("errors").One.Items.EqualTo(modelState.ToArray().First()));
        }
    }
}
