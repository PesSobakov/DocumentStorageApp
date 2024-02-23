using Bunit;
using Microsoft.Extensions.DependencyInjection;
using DocumentStorageApp.Services;
using DocumentStorageApp.Components;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using DocumentStorageApp.Components.Pages;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using AngleSharp.Dom;

namespace DocumentStorageAppTest
{
    public class AzureStorageTest : IAzureStorage
    {
        string name;
        Stream content;
        string email;

        public AzureStorageTest(string name, Stream content, string email)
        {
            this.name = name;
            this.content = content;
            this.email = email;
        }

        async public Task UploadAsync(string name, Stream content, string email)
        {
            Assert.Equal(this.name, name);
            byte[] content1 = new byte[this.content.Length];
            byte[] content2 = new byte[content.Length];
            this.content.Read(content1);
            content.Read(content2);
            Assert.Equal(content1, content2);
            Assert.Equal(this.email, email);
        }
    }

    public class UploadFileTest
    {
        [Fact]
        public void UploadFileSuccessfully()
        {
            string name = "test file name.docx";
            byte[] contentArray = [0, 1, 2, 3, 4, 5];
            Stream content = new MemoryStream(contentArray);
            string email = "123@123";

            using var ctx = new TestContext();

            ctx.Services.AddSingleton<IAzureStorage>(new AzureStorageTest(name, content, email));

            var component = ctx.RenderComponent<Home>();
            component.Find("input[name=\"Model.Email\"]").Change(email);
            component.FindComponent<InputFile>().UploadFiles(InputFileContent.CreateFromBinary(contentArray, name));
            component.Find("[type = \"submit\"]").Click();
            Assert.Empty(component.FindAll("div[class=\"validation-message\"]"));
        }

        [Fact]
        public void UploadFailedEmail()
        {
            string name = "test file name.docx";
            byte[] contentArray = [0, 1, 2, 3, 4, 5];
            Stream content = new MemoryStream(contentArray);
            string email = "123";

            using var ctx = new TestContext();

            ctx.Services.AddSingleton<IAzureStorage>(new AzureStorageTest(name, content, email));

            var component = ctx.RenderComponent<Home>();
            component.Find("input[name=\"Model.Email\"]").Change(email);
            component.FindComponent<InputFile>().UploadFiles(InputFileContent.CreateFromBinary(contentArray, name));
            component.Find("[type = \"submit\"]").Click();
            Assert.NotEmpty(component.FindAll("div[class=\"validation-message\"]").Where((x) => x.GetInnerText() == "Invalid email format"));
        }

        [Fact]
        public void UploadFailedFile()
        {
            string name = "test file name.txt";
            byte[] contentArray = [0, 1, 2, 3, 4, 5];
            Stream content = new MemoryStream(contentArray);
            string email = "123@123";

            using var ctx = new TestContext();

            ctx.Services.AddSingleton<IAzureStorage>(new AzureStorageTest(name, content, email));

            var component = ctx.RenderComponent<Home>();
            component.Find("input[name=\"Model.Email\"]").Change(email);
            component.FindComponent<InputFile>().UploadFiles(InputFileContent.CreateFromBinary(contentArray, name));
            component.Find("[type = \"submit\"]").Click();
            Assert.NotEmpty(component.FindAll("div[class=\"validation-message\"]").Where((x) => x.GetInnerText() == "Wrong extension, choose .docx file"));
        }
    }


}