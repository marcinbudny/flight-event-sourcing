using Xunit;

namespace FlightEventSourcing.Tests;

public class There_should_be_tests_here
{
    [Fact]
    public void but_instead_its_just_a_description()
    {
        Assert.Fail("Not implemented");
        
        /*
         * To limit time invested in this exercise, tests were skipped (which is a super bad practice and
         * they should have been actually written along with the code).
         *
         * The testing strategy I'd propose is to implement unit and integration tests.
         *
         * For unit tests, cover the code that represents significant logic in the application
         * - Avinor XML parsing
         * - ResolveUpdateToEvents method in the embedding
         * - event handlers in the embedding
         * - caching logic in the poller
         * - event handlers in the projection
         *
         * For integration tests, the approach would be to prepare a fixture that sets up the working version of
         * the service hosted on asp.net TestHost. As for the service's dependencies, they won't be mocked unless
         * necessary.
         *
         * Dependencies that could be used as-is:
         * - Dolittle runtime (hopefully, however the testing story is not documented)
         * - Mongo DB
         *
         * A strategy to provide clean environment for test execution is necessary. If the tests depend on externally
         * run test environment in docker compose, then the fixture needs to either clean databases or
         * run in separate scope (e.g. Dolittle's tenant). Alternatively, the test containers could be started along
         * with the test using something like https://github.com/testcontainers/testcontainers-dotnet
         *
         * Avinor's API needs to be mocked, to ensure deterministic testing. The endpoint is very simple and could
         * be mocked using WireMock.NET.
         *
         * Once the integration testing environment is ready, following tests can be implemented:
         * - full e2e test where flight data is polled, then retrieved
         *   by the test code from the flights API exposed by the service
         * - test similar to previous, but involving multiple polls with different data returned from the API
         * - behaviour on various errors returned from the Avinor API
         * 
         */
    }
}