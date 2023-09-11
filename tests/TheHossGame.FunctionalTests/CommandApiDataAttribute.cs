// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandApiDataAttribute.cs" company="Microsoft">
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
using AutoFixture.Xunit2;

public class CommandApiDataAttribute : AutoDataAttribute
{
    public CommandApiDataAttribute()
        : base(() => new Fixture()
            .Customize(new CompositeCustomization(
                new CommandApiCustomization())))
    {
    }
}