# XMPNG

XMPNG (Differential Motion PNG) is a simple lossless video codec, particularly well-suited for screencasts and screen sharing applications. This is a quick and dirty C#/.NET implementation from an old academic project.

The concept is pretty straightforward: the encoder writes the initial *I* frame as a full PNG image, then it writes each subsequent *P* frame as the PNG encoded XOR "difference" with respect of its preceding frame (i.e. P2 = Frame 2 XOR Frame 1).
This method allows to greatly reduce both the inter-frame temporal redundancy, by means of the fast and effective XOR "difference", and the intra-frame spatial redundancy, thanks to PNG's own 2D prediction filter.

![alt tag](http://i.imgur.com/SaEqSrQ.png)


The decoder than can reconstruct the exact, original frames by making a XOR operation between each *P* frame and the previous reconstructed frame (e.g. Frame 2 = P2 XOR Frame 1)

![alt tag](http://i.imgur.com/4eBBzYL.png)
