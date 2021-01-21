using Glasswall.IdentityManagementService.Api.ActionFilters;
using Glasswall.IdentityManagementService.Api.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using TestCommon;

namespace Service.Tests.Controllers.UsersControllerTests
{
    [TestFixture]
    public class ConstructorTests : UsersControllerTestBase
    {
        [Test]
        public void Constructs_With_Valid_Args()
        {
            ConstructorAssertions.ConstructsWithMockedParameters<UsersController>();
        }

        [Test]
        public void Throws_With_Null_Args()
        {
            ConstructorAssertions.ClassIsGuardedAgainstNull<UsersController>();
        }

        [Test]
        public void Correct_Attributes_Applied_To_Class()
        {
            var classInfo = typeof(UsersController);

            var attributes = classInfo.GetCustomAttributes(false);

            Assert.That(attributes, Has.Exactly(4).Items);
            Assert.That(attributes,
                Has.One.TypeOf<RouteAttribute>().With.Property(nameof(RouteAttribute.Template))
                    .EqualTo("api/v1/[controller]"));
            Assert.That(attributes,
                Has.One.TypeOf<ServiceFilterAttribute>().With.Property(nameof(ServiceFilterAttribute.ServiceType))
                    .EqualTo(typeof(ModelStateValidationActionFilterAttribute)));
            Assert.That(attributes, Has.One.TypeOf<ApiControllerAttribute>());
            Assert.That(attributes, Has.One.TypeOf<AuthorizeAttribute>());
        }

        [Test]
        public void Correct_Attributes_Applied_To_Authenticate()
        {
            var classInfo = typeof(UsersController);
            var methodInfo = classInfo.GetMethod(nameof(UsersController.Authenticate));

            var attributes = methodInfo?.GetCustomAttributes(false);

            Assert.That(attributes, Has.Exactly(4).Items);
            Assert.That(attributes,
                Has.One.TypeOf<HttpPostAttribute>().With.Property(nameof(HttpPostAttribute.Template))
                    .EqualTo("authenticate"));
            Assert.That(attributes, Has.One.TypeOf<AllowAnonymousAttribute>());
        }

        [Test]
        public void Correct_Attributes_Applied_To_New()
        {
            var classInfo = typeof(UsersController);
            var methodInfo = classInfo.GetMethod(nameof(UsersController.New));

            var attributes = methodInfo?.GetCustomAttributes(false);

            Assert.That(attributes, Has.Exactly(4).Items);
            Assert.That(attributes,
                Has.One.TypeOf<HttpPostAttribute>().With.Property(nameof(HttpPostAttribute.Template)).EqualTo("new"));
            Assert.That(attributes, Has.One.TypeOf<AllowAnonymousAttribute>());
        }

        [Test]
        public void Correct_Attributes_Applied_To_ForgotPassword()
        {
            var classInfo = typeof(UsersController);
            var methodInfo = classInfo.GetMethod(nameof(UsersController.ForgotPassword));

            var attributes = methodInfo?.GetCustomAttributes(false);

            Assert.That(attributes, Has.Exactly(4).Items);
            Assert.That(attributes,
                Has.One.TypeOf<HttpPostAttribute>().With.Property(nameof(HttpPostAttribute.Template))
                    .EqualTo("forgot-password"));
            Assert.That(attributes, Has.One.TypeOf<AllowAnonymousAttribute>());
        }

        [Test]
        public void Correct_Attributes_Applied_To_ValidateResetToken()
        {
            var classInfo = typeof(UsersController);
            var methodInfo = classInfo.GetMethod(nameof(UsersController.ValidateResetToken));

            var attributes = methodInfo?.GetCustomAttributes(false);

            Assert.That(attributes, Has.Exactly(4).Items);
            Assert.That(attributes,
                Has.One.TypeOf<HttpPostAttribute>().With.Property(nameof(HttpPostAttribute.Template))
                    .EqualTo("validate-reset-token"));
            Assert.That(attributes, Has.One.TypeOf<AllowAnonymousAttribute>());
        }

        [Test]
        public void Correct_Attributes_Applied_To_ResetPassword()
        {
            var classInfo = typeof(UsersController);
            var methodInfo = classInfo.GetMethod(nameof(UsersController.ResetPassword));

            var attributes = methodInfo?.GetCustomAttributes(false);

            Assert.That(attributes, Has.Exactly(4).Items);
            Assert.That(attributes,
                Has.One.TypeOf<HttpPostAttribute>().With.Property(nameof(HttpPostAttribute.Template))
                    .EqualTo("reset-password"));
            Assert.That(attributes, Has.One.TypeOf<AllowAnonymousAttribute>());
        }

        [Test]
        public void Correct_Attributes_Applied_To_GetAll()
        {
            var classInfo = typeof(UsersController);
            var methodInfo = classInfo.GetMethod(nameof(UsersController.GetAll));

            var attributes = methodInfo?.GetCustomAttributes(false);

            Assert.That(attributes, Has.Exactly(3).Items);
            Assert.That(attributes,
                Has.One.TypeOf<HttpGetAttribute>().With.Property(nameof(HttpGetAttribute.Template)));
        }

        [Test]
        public void Correct_Attributes_Applied_To_GetById()
        {
            var classInfo = typeof(UsersController);
            var methodInfo = classInfo.GetMethod(nameof(UsersController.GetById));

            var attributes = methodInfo?.GetCustomAttributes(false);

            Assert.That(attributes, Has.Exactly(3).Items);
            Assert.That(attributes,
                Has.One.TypeOf<HttpGetAttribute>().With.Property(nameof(HttpGetAttribute.Template)).EqualTo("{id}"));
        }

        [Test]
        public void Correct_Attributes_Applied_To_Update()
        {
            var classInfo = typeof(UsersController);
            var methodInfo = classInfo.GetMethod(nameof(UsersController.Update));

            var attributes = methodInfo?.GetCustomAttributes(false);

            Assert.That(attributes, Has.Exactly(3).Items);
            Assert.That(attributes,
                Has.One.TypeOf<HttpPutAttribute>().With.Property(nameof(HttpPutAttribute.Template)).EqualTo("{id}"));
        }

        [Test]
        public void Correct_Attributes_Applied_To_Delete()
        {
            var classInfo = typeof(UsersController);
            var methodInfo = classInfo.GetMethod(nameof(UsersController.Update));

            var attributes = methodInfo?.GetCustomAttributes(false);

            Assert.That(attributes, Has.Exactly(3).Items);
            Assert.That(attributes,
                Has.One.TypeOf<HttpPutAttribute>().With.Property(nameof(HttpPutAttribute.Template)).EqualTo("{id}"));
        }
    }
}