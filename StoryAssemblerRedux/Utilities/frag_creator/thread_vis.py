import networkx as nx
from pyvis.network import Network
import argparse

def assemble_graph_from_step_file(file_path):
    # Create a directed graph
    graph = nx.DiGraph()

    with open(file_path, 'r') as file:

        for line in file:
            line = line.strip()

            # Check if it's the beginning of a new fragment
            if line.startswith("Fragment"):
                current_node = line.split()[1]
                graph.add_node(current_node)
                
            # Check for GoToChoice to represent edges
            elif line.startswith("GoToChoice"):
                current_node = line.split()[1]
                destination_node = line.split()[2].replace(".", "")
                graph.add_edge(current_node, destination_node)

            elif line.startswith("Content"):
                content = line.split(":")[1].strip()
                graph.nodes[current_node]["content"] = content

    return graph

def visualize_graph(graph):
    nt = Network(height="800px", width="100%", directed=True, notebook=True)

    for node, data in graph.nodes(data=True):
        if "content" in data:
            # Set the content as a tooltip for the node
            nt.add_node(node, title=data["content"])
        else:
            nt.add_node(node)

    nt.from_nx(graph)
    nt.show_buttons(filter_=['physics'])
    nt.show("graph.html")

def assemble(step_file_path):
    # Assemble the graph
    graph = assemble_graph_from_step_file(step_file_path)

    # Visualize the graph
    visualize_graph(graph)

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="Assemble and visualize a directed graph from a Step file.")
    parser.add_argument("step_file_path", help="Path to the Step file")

    args = parser.parse_args()
    step_file_path = args.step_file_path

    assemble(step_file_path)