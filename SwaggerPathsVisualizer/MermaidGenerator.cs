using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace SwaggerPathsVisualizer;

public static class SwaggerMermaidGenerator
{
    public static string GenerateMermaidFromSwagger(string jsonContent, string diagramDirection = MermaidDiagramDirection.LeftRight)
    {
        var openApiDocument = JsonDocument.Parse(jsonContent);
        StringBuilder mermaid = new StringBuilder();
        mermaid.AppendLine($"flowchart {diagramDirection}");
        
        Dictionary<string, string> nodeIds = new Dictionary<string, string>();
        int nodeCounter = 0;
        
        // Add API Root node
        string rootNodeId = $"node{nodeCounter++}";
        nodeIds["API Root"] = rootNodeId;
        mermaid.AppendLine($"    {rootNodeId}([\"API Root\"])");

        var paths = openApiDocument.RootElement.GetProperty("paths");
        
        // Process paths and build node structure
        foreach (var pathProperty in paths.EnumerateObject())
        {
            var fullPath = pathProperty.Name;
            var segments = fullPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
            
            string currentPath = "";
            string parentPath = "API Root";
            
            // Process each path segment
            foreach (var segment in segments)
            {
                string currentSegment = segment;
                string displaySegment = segment;
                
                // Handle parameters in paths
                if (segment.StartsWith("{") && segment.EndsWith("}"))
                {
                    currentSegment = $"&lt;param&gt;{segment.Trim('{', '}')}";
                    displaySegment = $"&lt;param&gt;{segment.Trim('{', '}')}";
                }
                
                // Build the full path up to this segment
                currentPath += "/" + currentSegment;
                
                // Add node if it doesn't exist
                if (!nodeIds.ContainsKey(currentPath))
                {
                    string nodeId = $"node{nodeCounter++}";
                    nodeIds[currentPath] = nodeId;
                    mermaid.AppendLine($"    {nodeId}[\"{displaySegment}\"]");
                    
                    // Add edge from parent
                    mermaid.AppendLine($"    {nodeIds[parentPath]} --> {nodeId}");
                }
                
                parentPath = currentPath;
            }
            
            // Add HTTP methods as leaf nodes
            var methods = pathProperty.Value;
            foreach (var methodProperty in methods.EnumerateObject())
            {
                var method = methodProperty.Name.ToUpper();
                var operationId = "";
                if (methodProperty.Value.TryGetProperty("operationId", out var operationIdElement))
                {
                    operationId = operationIdElement.GetString();
                }
                else if (methodProperty.Value.TryGetProperty("summary", out var summaryElement))
                {
                    operationId = summaryElement.GetString();
                }
                
                var endpointName = $"{currentPath}|{method}|{operationId}";
                string methodNodeId = $"node{nodeCounter++}";
                nodeIds[endpointName] = methodNodeId;
                
                // Use different styling for method nodes
                mermaid.AppendLine($"    {methodNodeId}[\"{method} | {operationId}\"]");
                mermaid.AppendLine($"    {nodeIds[currentPath]} --> {methodNodeId}");
            }
        }
        
        return mermaid.ToString();
    }
}
