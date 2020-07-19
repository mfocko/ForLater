using RestSharp;
using RestSharp.Extensions;
using System;

using System.Collections.Generic;
using System.Text;

namespace ForLater.forges
{
    class GitLab : IForge
    {
        private RestClient client;
        private readonly string token;

        public string Host { get; }

        public GitLab(string token, string repository, string host = "gitlab.com")
        {
            Host = host;
            this.token = token;

            client = new RestClient($"https://{Host}/api/v4/projects/{Uri.EscapeUriString(repository)}/issues");
            client.AddDefaultHeader("Private-Token", token);
        }

        public override bool Equals(object? obj)
        {
            return obj is GitLab lab &&
                   token == lab.token;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(token);
        }

        public IForge Get()
        {
            throw new NotImplementedException();
        }

        public void GetIssue(Item todo)
        {
            var request = new RestRequest($"{todo.ID}");
            var response = client.Get(request);
        }

        public void PostIssue(Item todo, string description)
        {
            throw new NotImplementedException();
        }
    }
}
