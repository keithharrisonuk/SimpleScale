﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

using NLog;

using SimpleScale.HeadNode;
using SimpleScale.Common;
using SimpleScale.WorkerNode;
using SimpleScale.Queues;

namespace TestApp
{
    public partial class MainForm : Form
    {
        CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        MemoryQueueManager<Member, int> _queueManager = new MemoryQueueManager<Member, int>();
        HeadNode<Member, int> _headNode;
        private static Logger _logger;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _logger = LogManager.GetCurrentClassLogger();
            _headNode = new HeadNode<Member, int>(_queueManager);
            _headNode.BatchComplete += HeadNodeBatchComplete;
        }

        void HeadNodeBatchComplete(object sender, BatchCompleteEventArgs e)
        {
            _logger.Info("Batch " + e.BatchID + " complete.");
        }

        private void StartWorkerNodeButtonClick(object sender, EventArgs e)
        {
            var workerNode = new WorkerNode<Member, int>(_queueManager, new ValueMemberMapJob());
            workerNode.StartAsync(_cancellationTokenSource);
        }

        private void startHeadNodeButton_Click(object sender, EventArgs e)
        {
            var cancelationTokenSource = new CancellationTokenSource();
            _headNode.StartHeadNode(cancelationTokenSource);
            startHeadNodeButton.Enabled = false;
        }

        private void cancelButtonClick(object sender, EventArgs e)
        {
            _cancellationTokenSource.Cancel();
        }

        private void addBatchToQueueButtonClick(object sender, EventArgs e)
        {
            _headNode.RunBatch(GetBatch());
        }

        private void logTextBox_TextChanged(object sender, EventArgs e)
        {
            logTextBox.SelectionStart = logTextBox.Text.Length;
            logTextBox.ScrollToCaret();
        }

        public Batch<Member> GetBatch()
        {
            var jobDataList = new List<Member>{
                CreateMember("Tom"),
                CreateMember("Dick"),
                CreateMember("Harry"),
                CreateMember("Jane"),
                CreateMember("Anne")
            };

            return new Batch<Member>(jobDataList);
        }

        public Member CreateMember(string name)
        {
            return new Member { Name = name };
        }
    }
}
