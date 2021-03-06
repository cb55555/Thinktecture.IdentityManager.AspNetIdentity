﻿/*
 * Copyright 2014 Dominick Baier, Brock Allen
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Thinktecture.IdentityManager;

namespace Thinktecture.IdentityManager.Host
{
    public class AspNetIdentityIdentityManagerFactory
    {
        static AspNetIdentityIdentityManagerFactory()
        {
            System.Data.Entity.Database.SetInitializer(new System.Data.Entity.DropCreateDatabaseIfModelChanges<IdentityDbContext>());

            //System.Data.Entity.Database.SetInitializer(new System.Data.Entity.DropCreateDatabaseIfModelChanges<CustomDbContext>());
        }

        string _connString;
        public AspNetIdentityIdentityManagerFactory(string connString)
        {
            _connString = connString;
        }
        
        public IIdentityManagerService Create()
        {
            var db = new IdentityDbContext<IdentityUser>(_connString);
            var userStore = new UserStore<IdentityUser>(db);
            var userMgr = new Microsoft.AspNet.Identity.UserManager<IdentityUser>(userStore);
            var roleStore = new RoleStore<IdentityRole>(db);
            var roleMgr = new Microsoft.AspNet.Identity.RoleManager<IdentityRole>(roleStore);

            Thinktecture.IdentityManager.AspNetIdentity.AspNetIdentityManagerService<IdentityUser, string, IdentityRole, string> svc = null;
            svc = new Thinktecture.IdentityManager.AspNetIdentity.AspNetIdentityManagerService<IdentityUser, string, IdentityRole, string>(userMgr, roleMgr);

            // uncomment to add other properties that are mapped to claims
            //svc = new Thinktecture.IdentityManager.AspNetIdentity.AspNetIdentityManagerService<IdentityUser, string, IdentityRole, string>(userMgr, roleMgr, () =>
            //{
            //    var meta = svc.GetStandardMetadata();
            //    meta.UserMetadata.UpdateProperties =
            //        meta.UserMetadata.UpdateProperties.Union(new PropertyMetadata[]{
            //            svc.GetMetadataForClaim(Constants.ClaimTypes.Name, "Name")
            //        });
            //    return Task.FromResult(meta);
            //});

            var dispose = new DisposableIdentityManagerService(svc, db);
            return dispose;

            //var db = new CustomDbContext(_connString);
            //var userstore = new CustomUserStore(db);
            //var usermgr = new CustomUserManager(userstore);
            //var rolestore = new CustomRoleStore(db);
            //var rolemgr = new CustomRoleManager(rolestore);

            //var svc = new Thinktecture.IdentityManager.AspNetIdentity.AspNetIdentityManagerService<CustomUser, int, CustomRole, int>(usermgr, rolemgr);
            //var dispose = new DisposableIdentityManagerService(svc, db);
            //return dispose;
        }
    }
}