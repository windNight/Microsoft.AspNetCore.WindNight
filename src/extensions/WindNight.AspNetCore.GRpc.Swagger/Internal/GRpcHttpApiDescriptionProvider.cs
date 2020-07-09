// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Google.Api;
using Google.Protobuf.Reflection;
using GRpc.Shared.HttpApi;
using Microsoft.AspNetCore.GRpc.HttpApi;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Routing;

namespace Microsoft.AspNetCore.GRpc.Swagger.Internal
{
    internal class GRpcHttpApiDescriptionProvider : IApiDescriptionProvider
    {
        private readonly EndpointDataSource _endpointDataSource;

        public GRpcHttpApiDescriptionProvider(EndpointDataSource endpointDataSource)
        {
            _endpointDataSource = endpointDataSource;
        }

        // Executes after ASP.NET Core
        public int Order => -900;

        public void OnProvidersExecuting(ApiDescriptionProviderContext context)
        {
            var endpoints = _endpointDataSource.Endpoints;

            foreach (var endpoint in endpoints)
                if (endpoint is RouteEndpoint routeEndpoint)
                {
                    var gRpcHttpMetadata = endpoint.Metadata.GetMetadata<GRpcHttpMetadata>();

                    if (gRpcHttpMetadata != null)
                    {
                        var httpRule = gRpcHttpMetadata.HttpRule;
                        var methodDescriptor = gRpcHttpMetadata.MethodDescriptor;

                        if (ServiceDescriptorHelpers.TryResolvePattern(gRpcHttpMetadata.HttpRule, out var pattern,
                            out var verb))
                        {
                            var apiDescription = CreateApiDescription(routeEndpoint, httpRule, methodDescriptor,
                                pattern, verb);

                            context.Results.Add(apiDescription);
                        }
                    }
                }
        }

        public void OnProvidersExecuted(ApiDescriptionProviderContext context)
        {
            // no-op
        }

        private static ApiDescription CreateApiDescription(RouteEndpoint routeEndpoint, HttpRule httpRule,
            MethodDescriptor methodDescriptor, string pattern, string verb)
        {
            var apiDescription = new ApiDescription();
            apiDescription.HttpMethod = verb;
            apiDescription.ActionDescriptor = new ActionDescriptor
            {
                RouteValues = new Dictionary<string, string>
                {
                    // Swagger uses this to group endpoints together.
                    // Group methods together using the service name.
                    ["controller"] = methodDescriptor.Service.FullName
                }
            };
            apiDescription.RelativePath = pattern.TrimStart('/');
            apiDescription.SupportedRequestFormats.Add(new ApiRequestFormat {MediaType = "application/json"});
            apiDescription.SupportedResponseTypes.Add(new ApiResponseType
            {
                ApiResponseFormats = {new ApiResponseFormat {MediaType = "application/json"}},
                ModelMetadata =
                    new GRpcModelMetadata(ModelMetadataIdentity.ForType(methodDescriptor.OutputType.ClrType)),
                StatusCode = 200
            });

            var routeParameters =
                ServiceDescriptorHelpers.ResolveRouteParameterDescriptors(routeEndpoint.RoutePattern,
                    methodDescriptor.InputType);

            foreach (var routeParameter in routeParameters)
            {
                var field = routeParameter.Value.Last();

                apiDescription.ParameterDescriptions.Add(new ApiParameterDescription
                {
                    Name = routeParameter.Key,
                    ModelMetadata =
                        new GRpcModelMetadata(
                            ModelMetadataIdentity.ForType(MessageDescriptorHelpers.ResolveFieldType(field))),
                    Source = BindingSource.Path,
                    DefaultValue = string.Empty
                });
            }

            ServiceDescriptorHelpers.ResolveBodyDescriptor(httpRule.Body, methodDescriptor, out var bodyDescriptor,
                out _, out _);
            if (bodyDescriptor != null)
                apiDescription.ParameterDescriptions.Add(new ApiParameterDescription
                {
                    Name = "Input",
                    ModelMetadata = new GRpcModelMetadata(ModelMetadataIdentity.ForType(bodyDescriptor.ClrType)),
                    Source = BindingSource.Body
                });

            return apiDescription;
        }
    }
}