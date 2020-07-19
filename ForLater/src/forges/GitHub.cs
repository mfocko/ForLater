using System;
using System.Collections.Generic;
using System.Text;

namespace ForLater.forges {
    class GitHub : IForge {
        public string Host => "github.com";

        public IForge Get() {
            throw new NotImplementedException();
        }

        public void GetIssue(Item todo) {
            throw new NotImplementedException();
        }

        public void PostIssue(Item todo, string description) {
            throw new NotImplementedException();
        }
    }
}
