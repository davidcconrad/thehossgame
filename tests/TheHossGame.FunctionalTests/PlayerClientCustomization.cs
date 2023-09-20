// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandApiCustomization.cs" company="Microsoft">
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
//   THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR
//   OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
//   ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
//   OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace TheHossGame.FunctionalTests;

using AutoFixture;
using Xunit.Sdk;

public class PlayerClientCustomization : ICustomization
{
    #region ICustomization Members

    public void Customize(IFixture fixture)
    {
        var helper = new TestOutputHelper();
        fixture.Customize<PlayerClient>(f => f.FromFactory(() =>
        {
            var commandApiFactory = new CommandApiFactory(helper);

            var playerClient = new PlayerClient(
                commandApiFactory.CreateClient(), 
                Guid.NewGuid().ToString());
            playerClient.AuthorizeAsync().GetAwaiter().GetResult();
            return playerClient;
        }));
        fixture.Customize<IEnumerable<PlayerClient>>(f => f
            .FromFactory(() => 
                fixture.CreateMany<PlayerClient>(count: 4)));
    }

    #endregion
}

public class CommandApiCustomization : ICustomization
{
    #region ICustomization Members

    public void Customize(IFixture fixture)
    {
        var helper = new TestOutputHelper();
        fixture.Customize<CommandApiFactory>(f => f.FromFactory(() =>
        {
            var commandApiFactory = new CommandApiFactory(helper);

            return commandApiFactory;
        }));
    }

    #endregion
}

// public class ApiHttpClient : HttpClient
// {
//     /// <summary>
//     /// Initializes a new instance of the <see cref="ApiHttpClient"/> class.
//     /// </summary>
//     public ApiHttpClient(HttpClient client) : base(client.hand)
//     {
//         
//     }
// }