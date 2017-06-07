// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
namespace Microsoft.Azure.Management.Graph.RBAC.Fluent
{
    using System.Threading;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using Microsoft.Azure.Management.Graph.RBAC.Fluent.Models;
    using Microsoft.Azure.Management.ResourceManager.Fluent.Core.CollectionActions;
    using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
    using Microsoft.Rest;
    using System;
    using System.Linq;

    /// <summary>
    /// The implementation of Applications and its parent interfaces.
    /// </summary>
    public partial class ActiveDirectoryApplicationsImpl  :
        CreatableResources<Microsoft.Azure.Management.Graph.RBAC.Fluent.IActiveDirectoryApplication,Microsoft.Azure.Management.Graph.RBAC.Fluent.ActiveDirectoryApplicationImpl,Models.ApplicationInner>,
        IActiveDirectoryApplications,
        IHasManager<Microsoft.Azure.Management.Graph.RBAC.Fluent.GraphRbacManager>,
        IHasInner<Microsoft.Azure.Management.Graph.RBAC.Fluent.IApplicationsOperations>
    {
        private IApplicationsOperations innerCollection;
        private GraphRbacManager manager;
                public GraphRbacManager Manager()
        {
            return this.manager;
        }

                public override async Task DeleteByIdAsync(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            await innerCollection.DeleteAsync(id, cancellationToken);
        }

                public async Task<Microsoft.Azure.Management.Graph.RBAC.Fluent.IActiveDirectoryApplication> GetByNameAsync(string name, CancellationToken cancellationToken = default(CancellationToken))
        {
            IEnumerable<ApplicationInner> inners = await manager.Inner.Applications.ListAsync(string.Format("displayName eq '{0}'", name), cancellationToken);
            if (inners == null || !inners.Any())
            {
                inners = await manager.Inner.Applications.ListAsync(string.Format("appId eq '{0}'", name), cancellationToken);
            }
            if (inners == null || ! inners.Any())
            {
                return null;
            }
            else
            {
                return await new ActiveDirectoryApplicationImpl(inners.First(), manager).RefreshCredentialsAsync(cancellationToken);
            }
        }

                public IEnumerable<Microsoft.Azure.Management.Graph.RBAC.Fluent.IActiveDirectoryApplication> List()
        {
            Func<ApplicationInner, IActiveDirectoryApplication> converter = inner =>
            {
                return ((ActiveDirectoryApplicationImpl) WrapModel(inner)).RefreshCredentialsAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            };

            return Inner.List()
                        .AsContinuousCollection(link => Inner.ListNext(link))
                        .Select(inner => converter(inner));
        }

                public IApplicationsOperations Inner
        {
            get
            {
                return innerCollection;
            }
        }

                public ActiveDirectoryApplicationImpl GetById(string id)
        {
            return (ActiveDirectoryApplicationImpl)((ActiveDirectoryApplicationImpl)WrapModel(innerCollection.Get(id))).RefreshCredentialsAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }

                public async Task<Microsoft.Azure.Management.Graph.RBAC.Fluent.IActiveDirectoryApplication> GetByIdAsync(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await ((ActiveDirectoryApplicationImpl)WrapModel(await innerCollection.GetAsync(id, cancellationToken))).RefreshCredentialsAsync(cancellationToken);
        }

                public IActiveDirectoryApplication GetByName(string spn)
        {
            return GetByNameAsync(spn).ConfigureAwait(false).GetAwaiter().GetResult();
        }

                public ActiveDirectoryApplicationImpl Define(string name)
        {
            return WrapModel(name);
        }

                internal  ActiveDirectoryApplicationsImpl(GraphRbacManager graphRbacManager)
        {
            this.innerCollection = graphRbacManager.Inner.Applications;
            this.manager = graphRbacManager;

        }

                public async Task<Microsoft.Azure.Management.ResourceManager.Fluent.Core.IPagedCollection<IActiveDirectoryApplication>> ListAsync(bool loadAllPages = true, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await PagedCollection<IActiveDirectoryApplication, ApplicationInner>.LoadPageWithWrapModelAsync(
                async (cancellation) => await Inner.ListAsync(null, cancellation),
                Inner.ListNextAsync,
                async (inner, cancellation) => await new ActiveDirectoryApplicationImpl(inner, manager).RefreshCredentialsAsync(cancellation),
                loadAllPages, cancellationToken);
        }

                protected override IActiveDirectoryApplication WrapModel(ApplicationInner applicationInner)
        {
            if (applicationInner == null)
            {
                return null;
            }
            return new ActiveDirectoryApplicationImpl(applicationInner, manager);
        }

                protected override ActiveDirectoryApplicationImpl WrapModel(string name)
        {
            return new ActiveDirectoryApplicationImpl(new ApplicationInner
            {
                DisplayName = name
            }, manager);
        }

        public override void DeleteById(string id)
        {
            innerCollection.Delete(id);
        }
    }
}