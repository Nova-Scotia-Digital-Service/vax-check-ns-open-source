using Xunit;
using VaxCheckNS.Rules.Extensions;

namespace VaxCheckNS.Rules.Tests.Rule
{
    public class RuleTests
    {
        private readonly string TestData = "Helloworld, my name is computer.";
        private readonly string FailMessage = "YOU FAIL";
        private readonly string SecondFailMessage = "YOU DEFINITELY FAIL";


        [Fact]
        public void CanCreateRule()
        {
            // Arrange, Act, Assert
            Assert.NotNull(TestData.CreateRuleSet(s => s.Contains("Hello"), ""));
        }

        [Fact]
        public void CanAppendRule()
        {
            // Arrange, Act, Assert
            Assert.NotNull(TestData
                .CreateRuleSet(s => s.Contains("Hello"), "")
                .AppendRule(s => s.Contains("Hello"), ""));
        }

        [Fact]
        public void GivenSingleValidRuleStatement_RuleIsValid()
        {
            // Arrange,
            var rule = TestData.CreateRuleSet(s => s.Contains("Hello"), FailMessage);
            // Act,
            var result = rule.Validate(FailMessage);
            // Assert
            Assert.True(result.Success);
        }

        [Fact]
        public void GivenSingleInvalidRuleStatement_RuleIsInvalid()
        {
            // Arrange,
            var rule = TestData.CreateRuleSet(s => s.Contains("Hejjo"), FailMessage);
            // Act,
            var result = rule.Validate(FailMessage);
            // Assert
            Assert.True(result.Failure);
        }

        [Fact]
        public void GivenMultipleRuleStatementWithAllRuleStatementValid_RuleIsValid()
        {
            // Arrange,
            var rule = TestData.CreateRuleSet(s => s.Contains("Hello"), FailMessage)
                .AppendRule(s => s.Contains("computer"), SecondFailMessage);
            // Act,
            var result = rule.Validate(FailMessage);
            // Assert
            Assert.True(result.Success);
        }

        [Fact]
        public void GivenMultipleRuleStatementWithInitialRuleStatementInvalid_RuleIsInvalidAndContainsCorrectFailMessage()
        {
            // Arrange,
            var rule = TestData.CreateRuleSet(s => s.Contains("Hejjo"), FailMessage)
                .AppendRule(s => s.Contains("computer"), SecondFailMessage);
            // Act,
            var result = rule.Validate(FailMessage);
            // Assert
            Assert.True(result.Failure && result.Message == FailMessage);
        }

        [Fact]
        public void GivenMultipleRuleStatementWithAppendedRuleStatementInvalid_RuleIsInvalidAndContainsCorrectFailMessage()
        {
            // Arrange,
            var rule = TestData.CreateRuleSet(s => s.Contains("Hello"), FailMessage)
                .AppendRule(s => s.Contains("compuuuuuter"), SecondFailMessage);
            // Act,
            var result = rule.Validate(FailMessage);
            // Assert
            Assert.True(result.Failure && result.Message == SecondFailMessage);
        }

        [Fact]
        public void GivenMultipleRuleStatementWithBothRuleStatementInvalid_RuleIsInvalidAndContainsFirstFailsMessage()
        {
            // Arrange,
            var rule = TestData.CreateRuleSet(s => s.Contains("Hejjo"), FailMessage)
                .AppendRule(s => s.Contains("compuuuuuter"), SecondFailMessage);
            // Act,
            var result = rule.Validate(FailMessage);
            // Assert
            Assert.True(result.Failure && result.Message == FailMessage);
        }

        [Fact]
        public void GivenLongRuleStatementWithAllRuleStatementValid_RuleIsValid()
        {
            // Arrange,
            var rule = TestData.CreateRuleSet(s => s.Contains("Hello"), FailMessage)
                .AppendRule(s => s.Contains("computer"), SecondFailMessage)
                .AppendRule(s => s.Contains("name"), "")
                .AppendRule(s => s.Contains(" "), "")
                .AppendRule(s => s.Contains("name"), "")
                .AppendRule(s => s.Contains("computer"), "");
            // Act,
            var result = rule.Validate(FailMessage);
            // Assert
            Assert.True(result.Success);
        }

        [Fact]
        public void GivenLongRuleStatementWithOneRuleStatementInvalid_RuleIsInvalid()
        {
            // Arrange,
            var rule = TestData.CreateRuleSet(s => s.Contains("Hello"), FailMessage)
                .AppendRule(s => s.Contains("computer"), SecondFailMessage)
                .AppendRule(s => s.Contains("name"), "")
                .AppendRule(s => s.Contains(" "), "")
                .AppendRule(s => s.Contains("compute device"), "")
                .AppendRule(s => s.Contains("name"), "");
            // Act,
            var result = rule.Validate(FailMessage);
            // Assert
            Assert.True(result.Failure);
        }
    }
}
