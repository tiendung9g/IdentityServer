using ClientMVC.Models;
using CompanyEmployees.Client.Models;
using CompanyEmployees.Client.Services;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using System.Security.Claims;

namespace ClientMVC.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ICompanyHttpClient _companyHttpClient;
        public HomeController(ICompanyHttpClient companyHttpClient)
        {
            _companyHttpClient = companyHttpClient;
        }

        public async Task<IActionResult> Index()
        {
            var httpClient = await _companyHttpClient.GetClient();

            var response = await httpClient.GetAsync("api/companies").ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var companiesString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                var companyViewModel = JsonConvert.DeserializeObject<List<CompanyViewModel>>(companiesString).ToList();

                return View(companyViewModel);
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            throw new Exception($"Problem with fetching data from the API: {response.ReasonPhrase}");
        }

        //[Authorize(Roles = "Admin")]
        [Authorize(Policy = "CanCreateAndModifyData")]
        public async Task<IActionResult> Privacy()
        {
            var client = new HttpClient();
            var metaDataResponse = await client.GetDiscoveryDocumentAsync("https://localhost:5005");

            var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            var response = await client.GetUserInfoAsync(new UserInfoRequest
            {
                Address = metaDataResponse.UserInfoEndpoint,
                Token = accessToken
            });

            if (response.IsError)
            {
                throw new Exception("Problem while fetching data from the UserInfo endpoint", response.Exception);
            }

            var addressClaim = response.Claims.FirstOrDefault(c => c.Type.Equals("address"));

            User.AddIdentity(new ClaimsIdentity(new List<Claim> { new Claim(addressClaim.Type.ToString(), addressClaim.Value.ToString()) }));

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}