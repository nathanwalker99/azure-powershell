﻿using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;
using System.Linq;
using Microsoft.Azure.Commands.DataBox.Common;
using Microsoft.Azure.Management.DataBox.Models;
using Microsoft.Azure.Management.DataBox;
using Microsoft.Rest.Azure;
using System.Threading;

namespace MMicrosoft.Azure.Commands.DataBox.Common
{
    [Cmdlet(VerbsCommon.Get, "AzDataBoxJobs", DefaultParameterSetName = ListParameterSet), OutputType(typeof(JobResource))]
    public class GetDataBoxJobs : AzureDataBoxCmdletBase
    {
        private const string ListParameterSet = "ListParameterSet";
        private const string GetByNameParameterSet = "GetByNameParameterSet";
        private const string GetByResourceIdParameterSet = "GetByResourceIdParameterSet";

        public static string TenantId { get; internal set; }
        public static string SubscriptionId { get; internal set; }

        [Parameter(Mandatory = false, ParameterSetName = ListParameterSet)]
        [Parameter(Mandatory = true, ParameterSetName = GetByNameParameterSet)]
        [ValidateNotNullOrEmpty]
        public string ResourceGroupName { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = GetByNameParameterSet)]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, ParameterSetName = GetByResourceIdParameterSet)]
        [ValidateNotNullOrEmpty]
        public string ResourceId { get; set; }

        public override void ExecuteCmdlet()
        {
            //if (this.IsParameterBound(c => c.ResourceId))
            //{
            //    var resourceIdentifier = new ResourceIdentifier(this.ResourceId);
            //    this.ResourceGroupName = resourceIdentifier.ResourceGroupName;
            //    this.Name = resourceIdentifier.ResourceName;
            //}

            // Initializes a new instance of the DataBoxManagementClient class.

            this.DataBoxManagementClient.SubscriptionId = "05b5dd1c-793d-41de-be9f-6f9ed142f695";
            if (Name != null && string.IsNullOrWhiteSpace(Name))
            {
                throw new PSArgumentNullException("Name");
            }

            if (!string.IsNullOrEmpty(this.Name))
            {
                List<JobResource> result = new List<JobResource>();
                result.Add(JobsOperationsExtensions.Get(
                                this.DataBoxManagementClient.Jobs,
                                this.ResourceGroupName,
                                this.Name,
                                "details"));
                WriteObject(result);
            }
            else if (!string.IsNullOrEmpty(this.ResourceGroupName))
            {
                IPage<JobResource> jobPageList = null;
                List<JobResource> result = new List<JobResource>();
                
                do
                {
                    // Lists all the jobs available under resource group.
                    if (jobPageList == null)
                    {
                        jobPageList = JobsOperationsExtensions.ListByResourceGroup(
                                        this.DataBoxManagementClient.Jobs,
                                        this.ResourceGroupName);
                    }
                    else
                    {
                        jobPageList = JobsOperationsExtensions.ListByResourceGroupNext(
                                        this.DataBoxManagementClient.Jobs,
                                        jobPageList.NextPageLink);
                    }

                    result.AddRange(jobPageList.ToList());

                } while (!(string.IsNullOrEmpty(jobPageList.NextPageLink)));
                WriteObject(result, true);
            }
            else
            {
                 IPage<JobResource> jobPageList = null;
                 List<JobResource> result = new List<JobResource>();

                 do
                 {
                     // Lists all the jobs available under the subscription.
                     if (jobPageList == null)
                     {
                         jobPageList = JobsOperationsExtensions.List(
                                         this.DataBoxManagementClient.Jobs);
                     }
                     else
                     {
                         jobPageList = JobsOperationsExtensions.ListNext(
                                         this.DataBoxManagementClient.Jobs,
                                         jobPageList.NextPageLink);
                     }
                   
                     result.AddRange(jobPageList.ToList());

                 } while (!(string.IsNullOrEmpty(jobPageList.NextPageLink)));

                WriteObject(result, true);
            }
        }
    }

}
