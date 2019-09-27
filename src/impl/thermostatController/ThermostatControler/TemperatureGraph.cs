using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ThermostatControler
{
    public class TemperatureGraph
    {
        protected const int START_X = 0;
        
        protected System.Windows.Forms.Panel m_panel;
        protected int CLICK_TRESHOLD = 3;
        protected int m_padding = 25;
        // number of points
        protected int m_cntX;
        protected int m_cntY;
        // temperature from witch graph is starting
        protected int m_tempStart;
        protected int m_tempEnd;
        // size of canvas
        protected int m_width;
        protected int m_height;
        protected System.Drawing.Graphics m_graphics;
        protected System.Drawing.Pen m_penRed;
        protected System.Drawing.Pen m_penGray;
        protected System.Drawing.Pen m_penGreen;
        protected ArrayList m_points;

        /// <summary>
        /// Sets starting line
        /// </summary>
        /// <param name="panel">panel to draw on</param>
        /// <param name="x">num of points on x</param>
        /// <param name="y">num of points on y</param>
        public TemperatureGraph(Panel panel, int x, int y, int tempStart, int padding = 25)
        {
            this.m_panel = panel;
            m_cntX = x;
            m_cntY = y;
            m_tempStart = tempStart;
            m_tempEnd = tempStart + y;
            m_padding = padding;
            m_width = panel.Width - 2 * m_padding;
            m_height = panel.Height - 2 * m_padding;
            CLICK_TRESHOLD = Convert.ToInt32(m_width / m_cntX / 12);

            m_graphics = m_panel.CreateGraphics();
            m_penRed = new System.Drawing.Pen(System.Drawing.Color.Salmon, 2);
            m_penGray = new System.Drawing.Pen(System.Drawing.Color.LightGray, 2);
            m_penGreen = new System.Drawing.Pen(System.Drawing.Color.ForestGreen, 2);
            m_points = new ArrayList();
            // add starting temperature 20 C
            m_points.Add(ConvertPoint(new Point(m_padding, m_height / 2 + m_padding)));
        }

        public TemperatureGraph(Panel panel, int x, int y, int tempStart, ArrayList points, int padding = 25)
        {
            this.m_panel = panel;
            m_cntX = x;
            m_cntY = y;
            m_tempStart = tempStart;
            m_tempEnd = tempStart + y;
            m_padding = padding;
            m_width = panel.Width - 2 * m_padding;
            m_height = panel.Height - 2 * m_padding;
            CLICK_TRESHOLD = Convert.ToInt32(m_width / m_cntX / 12);

            m_graphics = m_panel.CreateGraphics();
            m_penRed = new System.Drawing.Pen(System.Drawing.Color.Salmon, 2);
            m_penGray = new System.Drawing.Pen(System.Drawing.Color.LightGray, 2);
            m_penGreen = new System.Drawing.Pen(System.Drawing.Color.ForestGreen, 2);
            m_points = new ArrayList();
            // add starting temperatures
            m_points = points;
        }

        public void Initialize()
        {
            m_panel.Controls.Clear();
            AddLabels();
            ReDraw();
        }

        public void DrawAxis()
        {
            m_graphics.DrawLine(m_penGray, m_padding, m_padding, m_padding, m_height + m_padding);
            m_graphics.DrawLine(m_penGray, m_padding, m_height + m_padding, m_width + m_padding, m_height + m_padding);
            Pen penGrayThin = new Pen(Color.LightGray, 1);

            int scale = m_height / m_cntY;
            for (int i = 0; i < m_height; i += scale)
            {
                m_graphics.DrawLine(penGrayThin, m_padding, m_padding + i, m_padding + m_width, m_padding + i);
            }
            scale = m_width / m_cntX;
            for (int i = 0; i <= m_width; i += scale)
            {
                m_graphics.DrawLine(penGrayThin, m_padding + i, m_padding, m_padding + i, m_padding + m_height);
            }
        }

        virtual public void AddLabels()
        {
            AddLabels(START_X, 1, m_tempStart, 1);
        }

        public void AddLabels(int startX, int incrementX, int startY, int incrementY)
        {
            double scale = 1.0 * m_width / m_cntX;
            for (int i = startX, j = 0; j <= m_cntX; i += incrementX, j += incrementX)
            {
                Label l = new Label();
                l.Text = i.ToString();
                l.AutoSize = true;
                l.Location = new Point((int)(j * scale + m_padding - 7), m_height + m_padding + 2);
                //Debug.WriteLine("AddLabels::coords: " + (i * 6 * m_scale + PADDING) + " x " + (m_height + PADDING));
                m_panel.Controls.Add(l);
            }

            scale = m_height / m_cntY;
            for (int i = startY + m_cntY, j = 0; j <= m_cntY; i -= incrementY, j += incrementY)
            {
                Label l = new Label();
                l.Text = i.ToString();
                l.AutoSize = true;
                l.Location = new Point(0, (int) (m_padding + j * scale - 6));
                //Debug.WriteLine("AddLabels::coords: " + 0 + " x " + (m_height + PADDING + (i - startY) * 10 * m_scale - 6));
                m_panel.Controls.Add(l);
            }
        }

        /// <summary>
        /// Draws points on canvas.
        /// </summary>
        public void DrawPoints()
        {
            for (int i = 0; i < m_points.Count - 1; i++)
            {
                // horizontal line
                m_graphics.DrawLine(m_penRed, PointLocation(i), new Point(PointLocation(i + 1).X, PointLocation(i).Y));
                // vertival line
                m_graphics.DrawLine(m_penRed, new Point(PointLocation(i + 1).X, PointLocation(i).Y), PointLocation(i + 1));
            }
            // last horizontal line
            if (m_points.Count > 0)
                m_graphics.DrawLine(m_penRed, PointLocation(m_points.Count - 1),
                                          new Point(m_width + m_padding, PointLocation(m_points.Count - 1).Y));
        }

        private void DrawRealTemperature()
        {
            double mult1X, mult1Y, mult2X, mult2Y;
            mult1X = .17;
            mult1Y = .67;
            mult2X = .41;
            mult2Y = .9;

            Point start, end, ctrl1, ctrl2;

            double segX = m_width / m_cntX;
            double segY = m_height / m_cntY;

            for (int i = 0; i < m_points.Count - 1; i++)
            {
                int height = ((Point)m_points[i]).Y - ((Point)m_points[i + 1]).Y;
                //int width = ((Point)m_points[i]).X - ((Point)m_points[i + 1]).X;

                double relativeH = height / segY;
                double relativeW;
                if (relativeH > 0)
                    relativeW = Math.Abs(2 * relativeH);
                else
                    relativeW = Math.Abs(.5 * relativeH);

                double width = relativeW * segX;

                if (((Point)m_points[i + 1]).X - width > 0)
                {
                    start = new Point(((Point)m_points[i + 1]).X, ((Point)m_points[i]).Y);
                    end = new Point((int)(((Point)m_points[i + 1]).X + width), ((Point)m_points[i + 1]).Y);

                    //start = new Point((int)(((Point)m_points[i + 1]).X - width), ((Point)m_points[i]).Y);
                    //end = new Point(((Point)m_points[i + 1]).X, ((Point)m_points[i + 1]).Y);
                }
                else
                {
                    start = new Point(0, ((Point)m_points[i]).Y);
                    end = new Point((int)width, ((Point)m_points[i + 1]).Y);
                }

                ctrl1 = new Point((int)(start.X + width * mult1X), (int)(start.Y - height * mult1Y));
                ctrl2 = new Point((int)(start.X + width * mult2X), (int)(start.Y - height * mult2Y));

                //Debug.WriteLine("Start");
                //Debug.WriteLine(start.ToString());
                //Debug.WriteLine(ctrl1.ToString());
                //Debug.WriteLine(ctrl2.ToString());
                //Debug.WriteLine(end.ToString());

                m_graphics.DrawBezier(m_penGreen, PointLocation(start), PointLocation(ctrl1), PointLocation(ctrl2), PointLocation(end));
            }
        }

        /// <summary>
        /// Calculates location on panel.
        /// </summary>
        /// <param name="idx">index of point in m_points array</param>
        /// <returns>instance of point</returns>
        private Point PointLocation(int idx)
        {
            return (PointLocation((Point)m_points[idx]));
        }

        private Point PointLocation(Point point)
        {
            return new Point(
                point.X + m_padding,
                (m_tempEnd * 10 - point.Y) * (m_height / m_cntY) / 10 + m_padding
                );
        }
        
        /// <summary>
        /// Adds point to graph if <paramref name="point"/> isn't present.
        /// Removes point from graph + checks the surounding for easier clicking.
        /// Moves point on same axis +- CLIK_TRESHOLD.
        /// </summary>
        /// <param name="point">Point to change in graph</param>
        public void ChangeLine(Point point)
        {
            if (!ValidPoint(point))
                return;

            Point p = ConvertPoint(point);
            // removes point
            //for (int i = p.X - CLICK_TRESHOLD; i <= p.X + CLICK_TRESHOLD; i++)
            //{
                for (int j = p.Y - CLICK_TRESHOLD / 2; j < p.Y + CLICK_TRESHOLD; j++)
                {
                    if (m_points.Contains(new Point(p.X, j)))
                    {
                        m_points.Remove(new Point(p.X, j));
                        m_points.Sort(Comparer<Point>.Create((a, b) => a.X - b.X));
                        ReDraw();
                        return;
                    }
                }
            //}

            // moves point on the same axis
            bool found = false;
            foreach (Point x in m_points)
            {
                for (int i = -CLICK_TRESHOLD; i <= CLICK_TRESHOLD; i++)
                {
                    if (x.X == p.X + i)
                    {
                        m_points.Remove(x);
                        found = true;
                        break;
                    }
                }
                if (found)
                    break;
            }
            m_points.Add(p);
            m_points.Sort(Comparer<Point>.Create((a, b) => a.X - b.X));
            ReDraw();
        }

        /// <summary>
        /// Adds set temperature to graph
        /// </summary>
        /// <param name="hours"></param>
        /// <param name="minutes"></param>
        /// <param name="temp"></param>
        public void AddNode(int hours, int minutes, double temp)
        {
            ChangeLine(PointLocation(ConvertNodeTosaveFormat(hours, minutes, temp)));
        }

        /// <summary>
        /// Converts node, to format that is saved.
        /// </summary>
        /// <param name="hours"></param>
        /// <param name="minutes"></param>
        /// <param name="temp"></param>
        /// <returns>New Point in save format</returns>
        private Point ConvertNodeTosaveFormat(int hours, int minutes, double temp)
        {
            double unitX;
            unitX = m_width / m_cntX;
            return new Point(
                        (int)((hours + minutes * 1.0 / 60) * unitX),
                        (int)(temp * 10)
                        );
        }

        /// <summary>
        /// Creates list if temperature nodes
        /// </summary>
        /// <returns>list of temperature nodes</returns>
        public List<TempNode> ExportPoints()
        {
            List<TempNode> res = new List<TempNode>();
            foreach (Point x in m_points)
            {
                res.Add(ExportNode(x));
            }
            return res;
        }

        /// <summary>
        /// Returns list of points.
        /// </summary>
        /// <returns>Returns list of points.</returns>
        public ArrayList GetPoints()
        {
            return m_points;
        }

        /// <summary>
        /// Creates one node
        /// </summary>
        /// <param name="x">Point to export</param>
        /// <returns>temperature node</returns>
        private TempNode ExportNode(Point x)
        {
            double len_x = m_width / m_cntX;
            double len_y = m_height / m_cntY;
            double time = Math.Floor(x.X / len_x) + Math.Floor(((x.X / len_x) - Math.Floor(x.X / len_x)) / (1 / 6.0)) / 10.0;
            double temp = x.Y / 10.0;

            if (temp < 10.0)
                temp = 10.0;

            if (time >= 24)
                time = 23.5;
            return new TempNode(time, temp);
        }

        /// <summary>
        /// Replaces curent graph with graph imported from schedule.
        /// </summary>
        /// <param name="schedule">Schedule from witch to import.</param>
        /// <param name="day">Day of schedule to import 0 - 6.</param>
        public void ImportScheduleDay(Schedule schedule, int day)
        {
            m_points.Clear();
            foreach (TempNode node in schedule.GetDay(day))
            {
                ImportNode(node);
            }
            ReDraw();
        }

        /// <summary>
        /// Imports node to graph.
        /// </summary>
        /// <param name="node">Node to import.</param>
        private void ImportNode(TempNode node)
        {
            m_points.Add(ConvertNodeTosaveFormat(
                            node.GetTimeHours(),
                            node.GetTimeMinutes(),
                            node.GetTemperatureDouble()
                            ));
        }

        /// <summary>
        /// Checks if the point is inside the graph.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private bool ValidPoint(Point p)
        {
            return p.X >= m_padding && p.X <= m_padding + m_width && p.Y >= m_padding && p.Y <= m_padding + m_height;
        }

        /// <summary>
        /// Converts point to temeperature.
        /// Leaves X as is.
        /// Temperature is 21.1 C => 210
        /// </summary>
        /// <param name="p"></param>
        private Point ConvertPoint(Point p)
        {
            return new Point(
                p.X - m_padding,
                Convert.ToInt32((1.0 * m_tempEnd - 1.0 * (p.Y - m_padding) / (m_height / m_cntY)) * 10)
                );
        }

        protected void Clear()
        {
            m_graphics.FillRectangle(new SolidBrush(Color.White), m_padding, 0, m_panel.Width, m_panel.Height);
        }

        protected virtual void ReDraw()
        {
            Clear();
            DrawAxis();
            DrawPoints();
            DrawRealTemperature();
        }
    }
}
