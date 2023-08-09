﻿using Microsoft.Msagl.Layout.Layered;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using TradeWarsData;

namespace TwvmLib.Controls;

/// <summary>
/// Interaction logic for Graph.xaml
/// </summary>
public partial class Graph : UserControl
{
    private Game? game;
    private Layers layers = new();


    public Graph()
    {
        InitializeComponent();

        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        //<Path Width="64" Height="64" Stretch="Uniform" Fill="Blue"
        //Data = "M8.660254,0 L17.320508,5 17.320508,15 8.660254,20 0,15 0,5 8.660254,0 Z" />

        //0   , 8.66     0  43.3
        //5   , 17.32    25  86.6
        //15  , 17.32    75  86.6
        //20  , 8.66     100 
        //15  , 0
        //Data="M 80,200 A 100,50 45 1 0 100,50"


        return;
        const int GRID_SIZE = 20;
        const int GRID_SPACING = 50;

        //System.Windows.Shapes.Path path = new();
        //Canvas.SetTop(line, 10);
        //Canvas.SetLeft(line, 10);

        for (int i = 0; i < GRID_SIZE; i++)
        {
            Line line = new();
            line.X1 = 0;
            line.X2 = GRID_SIZE * GRID_SPACING * 2;
            line.Y1 = i * GRID_SPACING;
            line.Y2 = line.Y1;
            line.StrokeThickness = 1;
            line.Stroke = System.Windows.Media.Brushes.Blue;

            line.SnapsToDevicePixels = true;
            line.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);
            canvas.Children.Add(line);

            Line line2 = new();
            line2.X1 = i * GRID_SPACING;
            line2.X2 = line2.X1 + GRID_SIZE * GRID_SPACING;
            line2.Y1 = 0;
            line2.Y2 = GRID_SIZE * GRID_SPACING;
            line2.StrokeThickness = 1;
            line2.Stroke = System.Windows.Media.Brushes.Blue;

            line2.SnapsToDevicePixels = true;
            line2.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);
            canvas.Children.Add(line2);

            Line line3 = new();
            line3.X1 = i * GRID_SPACING;
            line3.X2 = line2.X1 + GRID_SIZE * GRID_SPACING;
            line3.Y1 = GRID_SIZE * GRID_SPACING;
            line3.Y2 = 0;
            line3.StrokeThickness = 1;
            line3.Stroke = System.Windows.Media.Brushes.Blue;

            line3.SnapsToDevicePixels = true;
            line3.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);
            canvas.Children.Add(line3);


        }

    }

    public void MoveTo(int sector, Game? g)
    {
        if (g == null) return;
        game = g;

        LoadSectorMap();
        CreateGraph();
        LayoutSectors("Root");
        //LayoutSectors("Terra");




        Sector? cs = game.Sectors.FirstOrDefault(s => s.SectorId == sector);
        if (cs == null) return;

    }

    private void LoadSectorMap()
    {
        if (game == null) return;

        game.AddWarps(1, 2, 3, 4);
        game.AddWarps(2, 1, 3, 4);
        game.AddWarps(3, 1, 2, 4, 500);
        game.AddWarps(4, 1, 2, 3);
        game.AddWarps(500, 3, 9999);
        game.AddWarps(9999, 500, 18765);
        game.AddWarps(18765,9999);
    }

    private void CreateGraph()
    {
        Layer layer = layers.GetLayer("Root");
        layer.NewNode(1, new HexSector(1), 150, 600);
        layer.NewNode(3, new HexSector(3));
        layer.NewNode(500, new HexSector(500));
        layer.NewNode(9999, new HexSector(9999), 600, 150);
        layer.NewEdge(1,3);
        layer.NewEdge(3,500);
        layer.NewEdge(500,9999);

        //layer = layers.GetLayer("Terra");
        layer.NewNode(18765, new HexSector(18765));
        layer.NewNode(2, new HexSector(2));
        layer.NewNode(4, new HexSector(4));
        layer.NewEdge(18765,9999);
        layer.NewEdge(2,1);
        layer.NewEdge(2,3);
        layer.NewEdge(2, 4);
        layer.NewEdge(1, 4);

        //layers.Calculate();
    }

    private void LayoutSectors(string layout)
    {
        Layer? layer = layers.GetLayer("Root");
        if (layer == null) return;

        //var edges = layers.GetEdges(layout);
        //if (edges == null) return;
        foreach (var edge in layer.Edges)
        {
            //var sn = canvas.Children.OfType<HexSector>().FirstOrDefault(c => c.Sector == edge.Source.Sector);
            //var tn = canvas.Children.OfType<HexSector>().FirstOrDefault(c => c.Sector == edge.Target.Sector);
            //var sn = nodes.Find(n => n.Sector == edge.Source.Sector);
            //var tn = nodes.Find(n => n.Sector == edge.Target.Sector);
            //if (sn == null || tn == null) continue;


            WarpLine wl = new(edge.Source.HS as HexSector, edge.Target.HS as HexSector);
            //hs.AlertBrush = Brushes.Red;
            //Canvas.SetLeft(hs, node.X);
            //Canvas.SetTop(hs, node.Y);
            canvas.Children.Add(wl);

        }






        //var nodes = layers.GetNodes(layout);
        //if (nodes == null) return;
        foreach (var node in layer.Nodes)
        {
            //HexSector hs = new(node.Sector, node);
            var hs =  node.HS as HexSector;
            hs.AlertBrush = Brushes.Red;
            //Canvas.SetLeft(hs, node.X);
            //Canvas.SetTop(hs, node.Y);
            canvas.Children.Add(hs);
        }
        layer.NodeMoved += OnNodeMoved;
        layer.Run();

    }

    private void OnNodeMoved(object sender, NodeMovedEventArgs e)
    {
        //var child = canvas.Children.Select(c => c.Sector == 1);
        //canvas.Children.OfType<TextBlock>().FirstOrDefault();
        Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)(() =>
        {
            //var hs = canvas.Children.OfType<HexSector>().FirstOrDefault(c => c.Sector == e.Sector);
            var hs = e.HS as HexSector;
            if (hs == null) return;
            hs.X = e.X;
            hs.Y = e.Y;
            //hs.X = e.X;
            //Canvas.SetLeft(hs, e.X);
            //Canvas.SetTop(hs, e.Y);
        }));
    }
}


