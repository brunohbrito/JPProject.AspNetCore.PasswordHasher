
using Bogus;
using FluentAssertions;
using JpProject.AspNetCore.PasswordHasher.Argon2;
using JpProject.AspNetCore.PasswordHasher.Core;
using JpProject.AspNetCore.PasswordHasher.Tests.Fakers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using Xunit;

namespace JpProject.AspNetCore.PasswordHasher.Tests.Argon2
{
    public class Argon2Tests
    {
        private readonly Faker _faker;

        public Argon2Tests()
        {
            _faker = new Faker();
        }

        [Fact]
        public void ShouldBeTrueWhenPasswordStrengthSensitive()
        {
            var options = Options.Create(new ImprovedPasswordHasherOptions() { Strenght = PasswordHasherStrenght.Sensitive });

            var password = _faker.Internet.Password();
            var user = GenericUserFaker.GenerateUser().Generate();
            var argon2Hasher = new Argon2Id<GenericUser>(options);

            var hashedPass = argon2Hasher.HashPassword(user, password);

            argon2Hasher.VerifyHashedPassword(user, hashedPass, password).Should().Be(PasswordVerificationResult.Success);
        }

        [Fact]
        public void ShouldBeTrueWhenPasswordStrengthModerate()
        {
            var options = Options.Create(new ImprovedPasswordHasherOptions() { Strenght = PasswordHasherStrenght.Moderate });
            var password = _faker.Internet.Password();
            var user = GenericUserFaker.GenerateUser().Generate();
            var argon2Hasher = new Argon2Id<GenericUser>(options);

            var hashedPass = argon2Hasher.HashPassword(user, password);

            argon2Hasher.VerifyHashedPassword(user, hashedPass, password).Should().Be(PasswordVerificationResult.Success);
        }

        [Fact]
        public void ShouldBeTrueWhenPasswordStrengthInteractive()
        {
            var options = Options.Create(new ImprovedPasswordHasherOptions() { Strenght = PasswordHasherStrenght.Interactive });
            var password = _faker.Internet.Password();
            var user = GenericUserFaker.GenerateUser().Generate();
            var argon2Hasher = new Argon2Id<GenericUser>(options);

            var hashedPass = argon2Hasher.HashPassword(user, password);

            argon2Hasher.VerifyHashedPassword(user, hashedPass, password).Should().Be(PasswordVerificationResult.Success);
        }

        [Fact]
        public void ShouldBeTrueWhenPasswordWithCustomStrength()
        {
            var options = Options.Create(ImprovedPasswordHasherOptionsFaker.GenerateRandomOptions().Generate());
            var password = _faker.Internet.Password();
            var user = GenericUserFaker.GenerateUser().Generate();
            var argon2Hasher = new Argon2Id<GenericUser>(options);

            var hashedPass = argon2Hasher.HashPassword(user, password);

            argon2Hasher.VerifyHashedPassword(user, hashedPass, password).Should().Be(PasswordVerificationResult.Success);
        }
        [Fact]
        public void ShouldNotAcceptNullPasswordWhenHashingPassword()
        {
            var user = GenericUserFaker.GenerateUser().Generate();
            var argon2Hasher = new Argon2Id<GenericUser>();

            argon2Hasher.Invoking(i => i.HashPassword(user, null))
                .Should().Throw<ArgumentNullException>();

        }

        [Fact]
        public void ShouldNotAcceptNullUserWhenHashingPassword()
        {
            var password = _faker.Internet.Password();
            var argon2Hasher = new Argon2Id<GenericUser>();

            argon2Hasher.Invoking(i => i.HashPassword(null, password))
                .Should().Throw<ArgumentNullException>();

        }

        [Fact]
        public void ShouldNotAcceptNullPasswordWhenVerifyingPassword()
        {
            var options = Options.Create(new ImprovedPasswordHasherOptions() { Strenght = PasswordHasherStrenght.Interactive });
            var password = _faker.Internet.Password();
            var user = GenericUserFaker.GenerateUser().Generate();
            var argon2Hasher = new Argon2Id<GenericUser>(options);

            var hashedPass = argon2Hasher.HashPassword(user, password);

            argon2Hasher.Invoking(i => i.VerifyHashedPassword(user, hashedPass, null))
                .Should().Throw<ArgumentNullException>();

        }


        [Fact]
        public void ShouldNotAcceptNullHashedPasswordWhenVerifyingPassword()
        {
            var options = Options.Create(new ImprovedPasswordHasherOptions() { Strenght = PasswordHasherStrenght.Interactive });
            var password = _faker.Internet.Password();
            var user = GenericUserFaker.GenerateUser().Generate();
            var argon2Hasher = new Argon2Id<GenericUser>(options);


            argon2Hasher.Invoking(i => i.VerifyHashedPassword(user, null, password))
                .Should().Throw<ArgumentNullException>();

        }


        [Fact]
        public void ShouldNotAcceptNullUserWhenVerifyingPassword()
        {
            var options = Options.Create(new ImprovedPasswordHasherOptions() { Strenght = PasswordHasherStrenght.Interactive });
            var password = _faker.Internet.Password();
            var user = GenericUserFaker.GenerateUser().Generate();
            var argon2Hasher = new Argon2Id<GenericUser>(options);

            var hashedPass = argon2Hasher.HashPassword(user, password);

            argon2Hasher.Invoking(i => i.VerifyHashedPassword(null, hashedPass, password))
                .Should().Throw<ArgumentNullException>();

        }


        [Fact]
        public void ShouldMemLimitSameOfConfiguration()
        {
            var memLimit = _faker.Random.Int(1024, 1073741824);
            var services = new ServiceCollection();
            services.UpgradePasswordSecurity().WithMemLimit(memLimit).UseArgon2<GenericUser>();

            var provider = services.BuildServiceProvider();
            var passwordHasherOptions = (IOptions<ImprovedPasswordHasherOptions>)provider.GetService(typeof(IOptions<ImprovedPasswordHasherOptions>));

            passwordHasherOptions.Value.MemLimit.Should().Be(memLimit);
        }

        [Fact]
        public void ShouldOpsLimitSameOfConfiguration()
        {
            var opsLimit = _faker.Random.Long(3L, 16L);
            var services = new ServiceCollection();
            services.UpgradePasswordSecurity().WithOpsLimit(opsLimit).UseArgon2<GenericUser>();

            var provider = services.BuildServiceProvider();
            var passwordHasherOptions = (IOptions<ImprovedPasswordHasherOptions>)provider.GetService(typeof(IOptions<ImprovedPasswordHasherOptions>));

            passwordHasherOptions.Value.OpsLimit.Should().Be(opsLimit);
        }

        [Theory]
        [InlineData(PasswordHasherStrenght.Moderate)]
        [InlineData(PasswordHasherStrenght.Sensitive)]
        [InlineData(PasswordHasherStrenght.Interactive)]
        public void ShouldPasswordStrenghtSameOfConfiguration(PasswordHasherStrenght strenght)
        {
            var services = new ServiceCollection();
            services.UpgradePasswordSecurity().WithStrenghten(strenght).UseArgon2<GenericUser>();

            var provider = services.BuildServiceProvider();
            var passwordHasherOptions = (IOptions<ImprovedPasswordHasherOptions>)provider.GetService(typeof(IOptions<ImprovedPasswordHasherOptions>));

            passwordHasherOptions.Value.Strenght.Should().Be(strenght);
        }


        [Fact]
        public void ShouldConfigurationUseArgon2()
        {
            var services = new ServiceCollection();
            services.UpgradePasswordSecurity().UseArgon2<GenericUser>();

            var provider = services.BuildServiceProvider();
            var passwordHasher = (IPasswordHasher<GenericUser>)provider.GetService(typeof(IPasswordHasher<GenericUser>));

            passwordHasher.Should().BeOfType<Argon2Id<GenericUser>>();
        }
    }
}
