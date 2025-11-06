namespace WindowsFormsApp1
{
    partial class GeneralParameters
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GeneralParameters));
            this.lblCharHeight = new System.Windows.Forms.Label();
            this.txtCharHeight = new System.Windows.Forms.TextBox();
            this.lblCharWidth = new System.Windows.Forms.Label();
            this.txtCharWidth = new System.Windows.Forms.TextBox();
            this.lblCharSpeed = new System.Windows.Forms.Label();
            this.txtCharSpeed = new System.Windows.Forms.TextBox();
            this.lblCharSlide = new System.Windows.Forms.Label();
            this.txtCharSlide = new System.Windows.Forms.TextBox();
            this.txtCharWeight = new System.Windows.Forms.TextBox();
            this.lblCharWeight = new System.Windows.Forms.Label();
            this.lblCharGravity = new System.Windows.Forms.Label();
            this.txtCharGravity = new System.Windows.Forms.TextBox();
            this.txtCharFirstJumpHeight = new System.Windows.Forms.TextBox();
            this.lblCharFirstJumpHeight = new System.Windows.Forms.Label();
            this.txtCharFirstJumpDelay = new System.Windows.Forms.TextBox();
            this.lblCharFirstJumpDelay = new System.Windows.Forms.Label();
            this.txtCharSpeedAir = new System.Windows.Forms.TextBox();
            this.lblCharSpeedAir = new System.Windows.Forms.Label();
            this.txtCharSecondJumpHeight = new System.Windows.Forms.TextBox();
            this.lblCharSecondJumpHeight = new System.Windows.Forms.Label();
            this.txtCharDashDuration = new System.Windows.Forms.TextBox();
            this.lblCharDashDuration = new System.Windows.Forms.Label();
            this.txtCharDashDelay = new System.Windows.Forms.TextBox();
            this.lblCharDashDelay = new System.Windows.Forms.Label();
            this.txtCharBackDashDistance = new System.Windows.Forms.TextBox();
            this.lblCharBackDashDistance = new System.Windows.Forms.Label();
            this.txtCharBackDashWeight = new System.Windows.Forms.TextBox();
            this.lblCharBackDashWeight = new System.Windows.Forms.Label();
            this.txtCharBackDashHeight = new System.Windows.Forms.TextBox();
            this.lblCharBackDashHeight = new System.Windows.Forms.Label();
            this.txtCharBackDashHeight2 = new System.Windows.Forms.TextBox();
            this.lblCharBackDashHeight2 = new System.Windows.Forms.Label();
            this.txtCharStrength = new System.Windows.Forms.TextBox();
            this.lblCharStrength = new System.Windows.Forms.Label();
            this.txtCharDefense = new System.Windows.Forms.TextBox();
            this.lblCharDefense = new System.Windows.Forms.Label();
            this.txtCharDamageKnockback = new System.Windows.Forms.TextBox();
            this.lblCharDamageKnockback = new System.Windows.Forms.Label();
            this.txtCharAttackKnockback = new System.Windows.Forms.TextBox();
            this.lblCharAttackKnockback = new System.Windows.Forms.Label();
            this.txtCharStatus = new System.Windows.Forms.TextBox();
            this.lblCharStatus = new System.Windows.Forms.Label();
            this.txtCharQuantityProjectiles = new System.Windows.Forms.TextBox();
            this.lblCharQuantityProjectiles = new System.Windows.Forms.Label();
            this.txtCharHealingMultiplier = new System.Windows.Forms.TextBox();
            this.lblCharHealingMultiplier = new System.Windows.Forms.Label();
            this.txtCharChakraRecoverySpeed = new System.Windows.Forms.TextBox();
            this.lblCharChakraRecoverySpeed = new System.Windows.Forms.Label();
            this.txtCharDashSpeed = new System.Windows.Forms.TextBox();
            this.lblCharDashSpeed = new System.Windows.Forms.Label();
            this.txtCharDashDistance = new System.Windows.Forms.TextBox();
            this.lblCharDashDistance = new System.Windows.Forms.Label();
            this.grpMovementConfig = new System.Windows.Forms.GroupBox();
            this.lblRunningStartSpeed = new System.Windows.Forms.Label();
            this.txtRunningStartSpeed = new System.Windows.Forms.TextBox();
            this.grpGeneralConfig = new System.Windows.Forms.GroupBox();
            this.grpMovementAirConfig = new System.Windows.Forms.GroupBox();
            this.txtAirAtkYAdjLimit = new System.Windows.Forms.TextBox();
            this.lblAirAtkYAdjLimit = new System.Windows.Forms.Label();
            this.txtAirAtkXAdjLimit = new System.Windows.Forms.TextBox();
            this.lblAirAtkXAdjLimit = new System.Windows.Forms.Label();
            this.txtWallJmpDistanceLimit = new System.Windows.Forms.TextBox();
            this.lblWallJmpDistanceLimit = new System.Windows.Forms.Label();
            this.txtWallJmpRecoilDistance = new System.Windows.Forms.TextBox();
            this.lblWallJmpRecoilDistance = new System.Windows.Forms.Label();
            this.btnUpdateP1 = new System.Windows.Forms.Button();
            this.btnSaveELF = new System.Windows.Forms.Button();
            this.lblCharName = new System.Windows.Forms.Label();
            this.lblCharName2 = new System.Windows.Forms.Label();
            this.lblCharID = new System.Windows.Forms.Label();
            this.lblCharID2 = new System.Windows.Forms.Label();
            this.btnReset = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.grpMovementConfig.SuspendLayout();
            this.grpGeneralConfig.SuspendLayout();
            this.grpMovementAirConfig.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblCharHeight
            // 
            this.lblCharHeight.AutoSize = true;
            this.lblCharHeight.Location = new System.Drawing.Point(6, 100);
            this.lblCharHeight.Name = "lblCharHeight";
            this.lblCharHeight.Size = new System.Drawing.Size(103, 13);
            this.lblCharHeight.TabIndex = 7;
            this.lblCharHeight.Text = "Collision Box Height:";
            // 
            // txtCharHeight
            // 
            this.txtCharHeight.Location = new System.Drawing.Point(115, 97);
            this.txtCharHeight.Name = "txtCharHeight";
            this.txtCharHeight.Size = new System.Drawing.Size(51, 20);
            this.txtCharHeight.TabIndex = 8;
            // 
            // lblCharWidth
            // 
            this.lblCharWidth.AutoSize = true;
            this.lblCharWidth.Location = new System.Drawing.Point(361, 70);
            this.lblCharWidth.Name = "lblCharWidth";
            this.lblCharWidth.Size = new System.Drawing.Size(100, 13);
            this.lblCharWidth.TabIndex = 9;
            this.lblCharWidth.Text = "Collision Box Width:";
            // 
            // txtCharWidth
            // 
            this.txtCharWidth.Location = new System.Drawing.Point(466, 67);
            this.txtCharWidth.Name = "txtCharWidth";
            this.txtCharWidth.Size = new System.Drawing.Size(110, 20);
            this.txtCharWidth.TabIndex = 10;
            // 
            // lblCharSpeed
            // 
            this.lblCharSpeed.AutoSize = true;
            this.lblCharSpeed.Location = new System.Drawing.Point(235, 22);
            this.lblCharSpeed.Name = "lblCharSpeed";
            this.lblCharSpeed.Size = new System.Drawing.Size(84, 13);
            this.lblCharSpeed.TabIndex = 11;
            this.lblCharSpeed.Text = "Running Speed:";
            // 
            // txtCharSpeed
            // 
            this.txtCharSpeed.Location = new System.Drawing.Point(325, 19);
            this.txtCharSpeed.Name = "txtCharSpeed";
            this.txtCharSpeed.Size = new System.Drawing.Size(70, 20);
            this.txtCharSpeed.TabIndex = 12;
            // 
            // lblCharSlide
            // 
            this.lblCharSlide.AutoSize = true;
            this.lblCharSlide.Location = new System.Drawing.Point(416, 22);
            this.lblCharSlide.Name = "lblCharSlide";
            this.lblCharSlide.Size = new System.Drawing.Size(76, 13);
            this.lblCharSlide.TabIndex = 13;
            this.lblCharSlide.Text = "Running Slide:";
            // 
            // txtCharSlide
            // 
            this.txtCharSlide.Location = new System.Drawing.Point(498, 19);
            this.txtCharSlide.Name = "txtCharSlide";
            this.txtCharSlide.Size = new System.Drawing.Size(78, 20);
            this.txtCharSlide.TabIndex = 14;
            // 
            // txtCharWeight
            // 
            this.txtCharWeight.Location = new System.Drawing.Point(236, 95);
            this.txtCharWeight.Name = "txtCharWeight";
            this.txtCharWeight.Size = new System.Drawing.Size(111, 20);
            this.txtCharWeight.TabIndex = 15;
            // 
            // lblCharWeight
            // 
            this.lblCharWeight.AutoSize = true;
            this.lblCharWeight.Location = new System.Drawing.Point(186, 98);
            this.lblCharWeight.Name = "lblCharWeight";
            this.lblCharWeight.Size = new System.Drawing.Size(44, 13);
            this.lblCharWeight.TabIndex = 16;
            this.lblCharWeight.Text = "Weight:";
            // 
            // lblCharGravity
            // 
            this.lblCharGravity.AutoSize = true;
            this.lblCharGravity.Location = new System.Drawing.Point(361, 76);
            this.lblCharGravity.Name = "lblCharGravity";
            this.lblCharGravity.Size = new System.Drawing.Size(68, 13);
            this.lblCharGravity.TabIndex = 17;
            this.lblCharGravity.Text = "Char Gravity:";
            // 
            // txtCharGravity
            // 
            this.txtCharGravity.Location = new System.Drawing.Point(435, 73);
            this.txtCharGravity.Name = "txtCharGravity";
            this.txtCharGravity.Size = new System.Drawing.Size(141, 20);
            this.txtCharGravity.TabIndex = 18;
            // 
            // txtCharFirstJumpHeight
            // 
            this.txtCharFirstJumpHeight.Location = new System.Drawing.Point(284, 21);
            this.txtCharFirstJumpHeight.Name = "txtCharFirstJumpHeight";
            this.txtCharFirstJumpHeight.Size = new System.Drawing.Size(64, 20);
            this.txtCharFirstJumpHeight.TabIndex = 24;
            // 
            // lblCharFirstJumpHeight
            // 
            this.lblCharFirstJumpHeight.AutoSize = true;
            this.lblCharFirstJumpHeight.Location = new System.Drawing.Point(187, 24);
            this.lblCharFirstJumpHeight.Name = "lblCharFirstJumpHeight";
            this.lblCharFirstJumpHeight.Size = new System.Drawing.Size(91, 13);
            this.lblCharFirstJumpHeight.TabIndex = 23;
            this.lblCharFirstJumpHeight.Text = "First Jump Height:";
            // 
            // txtCharFirstJumpDelay
            // 
            this.txtCharFirstJumpDelay.Location = new System.Drawing.Point(99, 21);
            this.txtCharFirstJumpDelay.Name = "txtCharFirstJumpDelay";
            this.txtCharFirstJumpDelay.Size = new System.Drawing.Size(67, 20);
            this.txtCharFirstJumpDelay.TabIndex = 22;
            // 
            // lblCharFirstJumpDelay
            // 
            this.lblCharFirstJumpDelay.AutoSize = true;
            this.lblCharFirstJumpDelay.Location = new System.Drawing.Point(6, 24);
            this.lblCharFirstJumpDelay.Name = "lblCharFirstJumpDelay";
            this.lblCharFirstJumpDelay.Size = new System.Drawing.Size(87, 13);
            this.lblCharFirstJumpDelay.TabIndex = 21;
            this.lblCharFirstJumpDelay.Text = "First Jump Delay:";
            // 
            // txtCharSpeedAir
            // 
            this.txtCharSpeedAir.Location = new System.Drawing.Point(269, 73);
            this.txtCharSpeedAir.Name = "txtCharSpeedAir";
            this.txtCharSpeedAir.Size = new System.Drawing.Size(79, 20);
            this.txtCharSpeedAir.TabIndex = 20;
            // 
            // lblCharSpeedAir
            // 
            this.lblCharSpeedAir.AutoSize = true;
            this.lblCharSpeedAir.Location = new System.Drawing.Point(187, 76);
            this.lblCharSpeedAir.Name = "lblCharSpeedAir";
            this.lblCharSpeedAir.Size = new System.Drawing.Size(76, 13);
            this.lblCharSpeedAir.TabIndex = 19;
            this.lblCharSpeedAir.Text = "Gliding Speed:";
            // 
            // txtCharSecondJumpHeight
            // 
            this.txtCharSecondJumpHeight.Location = new System.Drawing.Point(476, 21);
            this.txtCharSecondJumpHeight.Name = "txtCharSecondJumpHeight";
            this.txtCharSecondJumpHeight.Size = new System.Drawing.Size(100, 20);
            this.txtCharSecondJumpHeight.TabIndex = 26;
            // 
            // lblCharSecondJumpHeight
            // 
            this.lblCharSecondJumpHeight.AutoSize = true;
            this.lblCharSecondJumpHeight.Location = new System.Drawing.Point(361, 24);
            this.lblCharSecondJumpHeight.Name = "lblCharSecondJumpHeight";
            this.lblCharSecondJumpHeight.Size = new System.Drawing.Size(109, 13);
            this.lblCharSecondJumpHeight.TabIndex = 25;
            this.lblCharSecondJumpHeight.Text = "Second Jump Height:";
            // 
            // txtCharDashDuration
            // 
            this.txtCharDashDuration.Location = new System.Drawing.Point(445, 47);
            this.txtCharDashDuration.Name = "txtCharDashDuration";
            this.txtCharDashDuration.Size = new System.Drawing.Size(131, 20);
            this.txtCharDashDuration.TabIndex = 30;
            // 
            // lblCharDashDuration
            // 
            this.lblCharDashDuration.AutoSize = true;
            this.lblCharDashDuration.Location = new System.Drawing.Point(361, 50);
            this.lblCharDashDuration.Name = "lblCharDashDuration";
            this.lblCharDashDuration.Size = new System.Drawing.Size(78, 13);
            this.lblCharDashDuration.TabIndex = 29;
            this.lblCharDashDuration.Text = "Dash Duration:";
            // 
            // txtCharDashDelay
            // 
            this.txtCharDashDelay.Location = new System.Drawing.Point(77, 47);
            this.txtCharDashDelay.Name = "txtCharDashDelay";
            this.txtCharDashDelay.Size = new System.Drawing.Size(89, 20);
            this.txtCharDashDelay.TabIndex = 28;
            // 
            // lblCharDashDelay
            // 
            this.lblCharDashDelay.AutoSize = true;
            this.lblCharDashDelay.Location = new System.Drawing.Point(6, 50);
            this.lblCharDashDelay.Name = "lblCharDashDelay";
            this.lblCharDashDelay.Size = new System.Drawing.Size(65, 13);
            this.lblCharDashDelay.TabIndex = 27;
            this.lblCharDashDelay.Text = "Dash Delay:";
            // 
            // txtCharBackDashDistance
            // 
            this.txtCharBackDashDistance.Location = new System.Drawing.Point(117, 45);
            this.txtCharBackDashDistance.Name = "txtCharBackDashDistance";
            this.txtCharBackDashDistance.Size = new System.Drawing.Size(105, 20);
            this.txtCharBackDashDistance.TabIndex = 36;
            // 
            // lblCharBackDashDistance
            // 
            this.lblCharBackDashDistance.AutoSize = true;
            this.lblCharBackDashDistance.Location = new System.Drawing.Point(6, 48);
            this.lblCharBackDashDistance.Name = "lblCharBackDashDistance";
            this.lblCharBackDashDistance.Size = new System.Drawing.Size(105, 13);
            this.lblCharBackDashDistance.TabIndex = 35;
            this.lblCharBackDashDistance.Text = "BackDash Distance:";
            // 
            // txtCharBackDashWeight
            // 
            this.txtCharBackDashWeight.Location = new System.Drawing.Point(331, 45);
            this.txtCharBackDashWeight.Name = "txtCharBackDashWeight";
            this.txtCharBackDashWeight.Size = new System.Drawing.Size(64, 20);
            this.txtCharBackDashWeight.TabIndex = 34;
            // 
            // lblCharBackDashWeight
            // 
            this.lblCharBackDashWeight.AutoSize = true;
            this.lblCharBackDashWeight.Location = new System.Drawing.Point(228, 48);
            this.lblCharBackDashWeight.Name = "lblCharBackDashWeight";
            this.lblCharBackDashWeight.Size = new System.Drawing.Size(97, 13);
            this.lblCharBackDashWeight.TabIndex = 33;
            this.lblCharBackDashWeight.Text = "BackDash Weight:";
            // 
            // txtCharBackDashHeight
            // 
            this.txtCharBackDashHeight.Location = new System.Drawing.Point(509, 45);
            this.txtCharBackDashHeight.Name = "txtCharBackDashHeight";
            this.txtCharBackDashHeight.Size = new System.Drawing.Size(67, 20);
            this.txtCharBackDashHeight.TabIndex = 32;
            // 
            // lblCharBackDashHeight
            // 
            this.lblCharBackDashHeight.AutoSize = true;
            this.lblCharBackDashHeight.Location = new System.Drawing.Point(409, 48);
            this.lblCharBackDashHeight.Name = "lblCharBackDashHeight";
            this.lblCharBackDashHeight.Size = new System.Drawing.Size(94, 13);
            this.lblCharBackDashHeight.TabIndex = 31;
            this.lblCharBackDashHeight.Text = "BackDash Height:";
            // 
            // txtCharBackDashHeight2
            // 
            this.txtCharBackDashHeight2.Location = new System.Drawing.Point(112, 71);
            this.txtCharBackDashHeight2.Name = "txtCharBackDashHeight2";
            this.txtCharBackDashHeight2.Size = new System.Drawing.Size(54, 20);
            this.txtCharBackDashHeight2.TabIndex = 38;
            // 
            // lblCharBackDashHeight2
            // 
            this.lblCharBackDashHeight2.AutoSize = true;
            this.lblCharBackDashHeight2.Location = new System.Drawing.Point(6, 74);
            this.lblCharBackDashHeight2.Name = "lblCharBackDashHeight2";
            this.lblCharBackDashHeight2.Size = new System.Drawing.Size(100, 13);
            this.lblCharBackDashHeight2.TabIndex = 37;
            this.lblCharBackDashHeight2.Text = "BackDash Height2:";
            // 
            // txtCharStrength
            // 
            this.txtCharStrength.Location = new System.Drawing.Point(62, 19);
            this.txtCharStrength.Name = "txtCharStrength";
            this.txtCharStrength.Size = new System.Drawing.Size(104, 20);
            this.txtCharStrength.TabIndex = 40;
            // 
            // lblCharStrength
            // 
            this.lblCharStrength.AutoSize = true;
            this.lblCharStrength.Location = new System.Drawing.Point(6, 24);
            this.lblCharStrength.Name = "lblCharStrength";
            this.lblCharStrength.Size = new System.Drawing.Size(50, 13);
            this.lblCharStrength.TabIndex = 39;
            this.lblCharStrength.Text = "Strength:";
            // 
            // txtCharDefense
            // 
            this.txtCharDefense.Location = new System.Drawing.Point(243, 17);
            this.txtCharDefense.Name = "txtCharDefense";
            this.txtCharDefense.Size = new System.Drawing.Size(104, 20);
            this.txtCharDefense.TabIndex = 42;
            // 
            // lblCharDefense
            // 
            this.lblCharDefense.AutoSize = true;
            this.lblCharDefense.Location = new System.Drawing.Point(187, 22);
            this.lblCharDefense.Name = "lblCharDefense";
            this.lblCharDefense.Size = new System.Drawing.Size(50, 13);
            this.lblCharDefense.TabIndex = 41;
            this.lblCharDefense.Text = "Defense:";
            // 
            // txtCharDamageKnockback
            // 
            this.txtCharDamageKnockback.Location = new System.Drawing.Point(117, 71);
            this.txtCharDamageKnockback.Name = "txtCharDamageKnockback";
            this.txtCharDamageKnockback.Size = new System.Drawing.Size(49, 20);
            this.txtCharDamageKnockback.TabIndex = 44;
            // 
            // lblCharDamageKnockback
            // 
            this.lblCharDamageKnockback.AutoSize = true;
            this.lblCharDamageKnockback.Location = new System.Drawing.Point(6, 74);
            this.lblCharDamageKnockback.Name = "lblCharDamageKnockback";
            this.lblCharDamageKnockback.Size = new System.Drawing.Size(108, 13);
            this.lblCharDamageKnockback.TabIndex = 43;
            this.lblCharDamageKnockback.Text = "Damage Knockback:";
            // 
            // txtCharAttackKnockback
            // 
            this.txtCharAttackKnockback.Location = new System.Drawing.Point(291, 69);
            this.txtCharAttackKnockback.Name = "txtCharAttackKnockback";
            this.txtCharAttackKnockback.Size = new System.Drawing.Size(56, 20);
            this.txtCharAttackKnockback.TabIndex = 46;
            // 
            // lblCharAttackKnockback
            // 
            this.lblCharAttackKnockback.AutoSize = true;
            this.lblCharAttackKnockback.Location = new System.Drawing.Point(187, 72);
            this.lblCharAttackKnockback.Name = "lblCharAttackKnockback";
            this.lblCharAttackKnockback.Size = new System.Drawing.Size(99, 13);
            this.lblCharAttackKnockback.TabIndex = 45;
            this.lblCharAttackKnockback.Text = "Attack Knockback:";
            // 
            // txtCharStatus
            // 
            this.txtCharStatus.Location = new System.Drawing.Point(119, 45);
            this.txtCharStatus.Name = "txtCharStatus";
            this.txtCharStatus.Size = new System.Drawing.Size(47, 20);
            this.txtCharStatus.TabIndex = 48;
            // 
            // lblCharStatus
            // 
            this.lblCharStatus.AutoSize = true;
            this.lblCharStatus.Location = new System.Drawing.Point(6, 48);
            this.lblCharStatus.Name = "lblCharStatus";
            this.lblCharStatus.Size = new System.Drawing.Size(107, 13);
            this.lblCharStatus.TabIndex = 47;
            this.lblCharStatus.Text = "Item Status Multiplier:";
            // 
            // txtCharQuantityProjectiles
            // 
            this.txtCharQuantityProjectiles.Location = new System.Drawing.Point(476, 41);
            this.txtCharQuantityProjectiles.Name = "txtCharQuantityProjectiles";
            this.txtCharQuantityProjectiles.Size = new System.Drawing.Size(100, 20);
            this.txtCharQuantityProjectiles.TabIndex = 50;
            // 
            // lblCharQuantityProjectiles
            // 
            this.lblCharQuantityProjectiles.AutoSize = true;
            this.lblCharQuantityProjectiles.Location = new System.Drawing.Point(360, 44);
            this.lblCharQuantityProjectiles.Name = "lblCharQuantityProjectiles";
            this.lblCharQuantityProjectiles.Size = new System.Drawing.Size(110, 13);
            this.lblCharQuantityProjectiles.TabIndex = 49;
            this.lblCharQuantityProjectiles.Text = "Number of Projectiles:";
            // 
            // txtCharHealingMultiplier
            // 
            this.txtCharHealingMultiplier.Location = new System.Drawing.Point(280, 43);
            this.txtCharHealingMultiplier.Name = "txtCharHealingMultiplier";
            this.txtCharHealingMultiplier.Size = new System.Drawing.Size(67, 20);
            this.txtCharHealingMultiplier.TabIndex = 52;
            // 
            // lblCharHealingMultiplier
            // 
            this.lblCharHealingMultiplier.AutoSize = true;
            this.lblCharHealingMultiplier.Location = new System.Drawing.Point(187, 46);
            this.lblCharHealingMultiplier.Name = "lblCharHealingMultiplier";
            this.lblCharHealingMultiplier.Size = new System.Drawing.Size(90, 13);
            this.lblCharHealingMultiplier.TabIndex = 51;
            this.lblCharHealingMultiplier.Text = "Healing Multiplier:";
            // 
            // txtCharChakraRecoverySpeed
            // 
            this.txtCharChakraRecoverySpeed.Location = new System.Drawing.Point(493, 15);
            this.txtCharChakraRecoverySpeed.Name = "txtCharChakraRecoverySpeed";
            this.txtCharChakraRecoverySpeed.Size = new System.Drawing.Size(83, 20);
            this.txtCharChakraRecoverySpeed.TabIndex = 54;
            // 
            // lblCharChakraRecoverySpeed
            // 
            this.lblCharChakraRecoverySpeed.AutoSize = true;
            this.lblCharChakraRecoverySpeed.Location = new System.Drawing.Point(360, 19);
            this.lblCharChakraRecoverySpeed.Name = "lblCharChakraRecoverySpeed";
            this.lblCharChakraRecoverySpeed.Size = new System.Drawing.Size(127, 13);
            this.lblCharChakraRecoverySpeed.TabIndex = 53;
            this.lblCharChakraRecoverySpeed.Text = "Chakra Recovery Speed:";
            // 
            // txtCharDashSpeed
            // 
            this.txtCharDashSpeed.Location = new System.Drawing.Point(262, 47);
            this.txtCharDashSpeed.Name = "txtCharDashSpeed";
            this.txtCharDashSpeed.Size = new System.Drawing.Size(86, 20);
            this.txtCharDashSpeed.TabIndex = 56;
            // 
            // lblCharDashSpeed
            // 
            this.lblCharDashSpeed.AutoSize = true;
            this.lblCharDashSpeed.Location = new System.Drawing.Point(187, 50);
            this.lblCharDashSpeed.Name = "lblCharDashSpeed";
            this.lblCharDashSpeed.Size = new System.Drawing.Size(69, 13);
            this.lblCharDashSpeed.TabIndex = 55;
            this.lblCharDashSpeed.Text = "Dash Speed:";
            // 
            // txtCharDashDistance
            // 
            this.txtCharDashDistance.Location = new System.Drawing.Point(92, 73);
            this.txtCharDashDistance.Name = "txtCharDashDistance";
            this.txtCharDashDistance.Size = new System.Drawing.Size(74, 20);
            this.txtCharDashDistance.TabIndex = 58;
            // 
            // lblCharDashDistance
            // 
            this.lblCharDashDistance.AutoSize = true;
            this.lblCharDashDistance.Location = new System.Drawing.Point(6, 76);
            this.lblCharDashDistance.Name = "lblCharDashDistance";
            this.lblCharDashDistance.Size = new System.Drawing.Size(80, 13);
            this.lblCharDashDistance.TabIndex = 57;
            this.lblCharDashDistance.Text = "Dash Distance:";
            // 
            // grpMovementConfig
            // 
            this.grpMovementConfig.Controls.Add(this.lblRunningStartSpeed);
            this.grpMovementConfig.Controls.Add(this.txtRunningStartSpeed);
            this.grpMovementConfig.Controls.Add(this.lblCharSlide);
            this.grpMovementConfig.Controls.Add(this.txtCharSlide);
            this.grpMovementConfig.Controls.Add(this.txtCharBackDashHeight2);
            this.grpMovementConfig.Controls.Add(this.lblCharBackDashHeight2);
            this.grpMovementConfig.Controls.Add(this.lblCharSpeed);
            this.grpMovementConfig.Controls.Add(this.txtCharBackDashDistance);
            this.grpMovementConfig.Controls.Add(this.lblCharBackDashDistance);
            this.grpMovementConfig.Controls.Add(this.txtCharSpeed);
            this.grpMovementConfig.Controls.Add(this.txtCharBackDashWeight);
            this.grpMovementConfig.Controls.Add(this.lblCharBackDashWeight);
            this.grpMovementConfig.Controls.Add(this.txtCharBackDashHeight);
            this.grpMovementConfig.Controls.Add(this.lblCharBackDashHeight);
            this.grpMovementConfig.Location = new System.Drawing.Point(1, 152);
            this.grpMovementConfig.Name = "grpMovementConfig";
            this.grpMovementConfig.Size = new System.Drawing.Size(604, 104);
            this.grpMovementConfig.TabIndex = 59;
            this.grpMovementConfig.TabStop = false;
            this.grpMovementConfig.Text = "Movement Parameters:";
            this.grpMovementConfig.Visible = false;
            // 
            // lblRunningStartSpeed
            // 
            this.lblRunningStartSpeed.AutoSize = true;
            this.lblRunningStartSpeed.Location = new System.Drawing.Point(6, 22);
            this.lblRunningStartSpeed.Name = "lblRunningStartSpeed";
            this.lblRunningStartSpeed.Size = new System.Drawing.Size(109, 13);
            this.lblRunningStartSpeed.TabIndex = 39;
            this.lblRunningStartSpeed.Text = "Running Start Speed:";
            // 
            // txtRunningStartSpeed
            // 
            this.txtRunningStartSpeed.Location = new System.Drawing.Point(121, 19);
            this.txtRunningStartSpeed.Name = "txtRunningStartSpeed";
            this.txtRunningStartSpeed.Size = new System.Drawing.Size(108, 20);
            this.txtRunningStartSpeed.TabIndex = 40;
            // 
            // grpGeneralConfig
            // 
            this.grpGeneralConfig.Controls.Add(this.lblCharHeight);
            this.grpGeneralConfig.Controls.Add(this.txtCharHeight);
            this.grpGeneralConfig.Controls.Add(this.txtCharWeight);
            this.grpGeneralConfig.Controls.Add(this.lblCharWidth);
            this.grpGeneralConfig.Controls.Add(this.lblCharWeight);
            this.grpGeneralConfig.Controls.Add(this.txtCharWidth);
            this.grpGeneralConfig.Controls.Add(this.txtCharChakraRecoverySpeed);
            this.grpGeneralConfig.Controls.Add(this.lblCharStrength);
            this.grpGeneralConfig.Controls.Add(this.lblCharChakraRecoverySpeed);
            this.grpGeneralConfig.Controls.Add(this.txtCharStrength);
            this.grpGeneralConfig.Controls.Add(this.txtCharHealingMultiplier);
            this.grpGeneralConfig.Controls.Add(this.lblCharDefense);
            this.grpGeneralConfig.Controls.Add(this.lblCharHealingMultiplier);
            this.grpGeneralConfig.Controls.Add(this.txtCharDefense);
            this.grpGeneralConfig.Controls.Add(this.txtCharQuantityProjectiles);
            this.grpGeneralConfig.Controls.Add(this.lblCharDamageKnockback);
            this.grpGeneralConfig.Controls.Add(this.lblCharQuantityProjectiles);
            this.grpGeneralConfig.Controls.Add(this.txtCharDamageKnockback);
            this.grpGeneralConfig.Controls.Add(this.txtCharStatus);
            this.grpGeneralConfig.Controls.Add(this.lblCharAttackKnockback);
            this.grpGeneralConfig.Controls.Add(this.lblCharStatus);
            this.grpGeneralConfig.Controls.Add(this.txtCharAttackKnockback);
            this.grpGeneralConfig.Location = new System.Drawing.Point(1, 32);
            this.grpGeneralConfig.Name = "grpGeneralConfig";
            this.grpGeneralConfig.Size = new System.Drawing.Size(604, 124);
            this.grpGeneralConfig.TabIndex = 60;
            this.grpGeneralConfig.TabStop = false;
            this.grpGeneralConfig.Text = "General Parameters:";
            this.grpGeneralConfig.Visible = false;
            // 
            // grpMovementAirConfig
            // 
            this.grpMovementAirConfig.Controls.Add(this.txtAirAtkYAdjLimit);
            this.grpMovementAirConfig.Controls.Add(this.lblCharSpeedAir);
            this.grpMovementAirConfig.Controls.Add(this.lblAirAtkYAdjLimit);
            this.grpMovementAirConfig.Controls.Add(this.txtCharDashDelay);
            this.grpMovementAirConfig.Controls.Add(this.txtAirAtkXAdjLimit);
            this.grpMovementAirConfig.Controls.Add(this.txtCharFirstJumpHeight);
            this.grpMovementAirConfig.Controls.Add(this.lblAirAtkXAdjLimit);
            this.grpMovementAirConfig.Controls.Add(this.txtCharDashDistance);
            this.grpMovementAirConfig.Controls.Add(this.txtWallJmpDistanceLimit);
            this.grpMovementAirConfig.Controls.Add(this.lblCharSecondJumpHeight);
            this.grpMovementAirConfig.Controls.Add(this.lblWallJmpDistanceLimit);
            this.grpMovementAirConfig.Controls.Add(this.txtCharDashDuration);
            this.grpMovementAirConfig.Controls.Add(this.txtWallJmpRecoilDistance);
            this.grpMovementAirConfig.Controls.Add(this.lblCharFirstJumpHeight);
            this.grpMovementAirConfig.Controls.Add(this.lblWallJmpRecoilDistance);
            this.grpMovementAirConfig.Controls.Add(this.txtCharSecondJumpHeight);
            this.grpMovementAirConfig.Controls.Add(this.lblCharGravity);
            this.grpMovementAirConfig.Controls.Add(this.txtCharGravity);
            this.grpMovementAirConfig.Controls.Add(this.txtCharFirstJumpDelay);
            this.grpMovementAirConfig.Controls.Add(this.lblCharDashDistance);
            this.grpMovementAirConfig.Controls.Add(this.lblCharDashDuration);
            this.grpMovementAirConfig.Controls.Add(this.lblCharFirstJumpDelay);
            this.grpMovementAirConfig.Controls.Add(this.lblCharDashSpeed);
            this.grpMovementAirConfig.Controls.Add(this.lblCharDashDelay);
            this.grpMovementAirConfig.Controls.Add(this.txtCharDashSpeed);
            this.grpMovementAirConfig.Controls.Add(this.txtCharSpeedAir);
            this.grpMovementAirConfig.Location = new System.Drawing.Point(1, 246);
            this.grpMovementAirConfig.Name = "grpMovementAirConfig";
            this.grpMovementAirConfig.Size = new System.Drawing.Size(604, 158);
            this.grpMovementAirConfig.TabIndex = 61;
            this.grpMovementAirConfig.TabStop = false;
            this.grpMovementAirConfig.Text = "Movement (Air) Config:";
            this.grpMovementAirConfig.Visible = false;
            // 
            // txtAirAtkYAdjLimit
            // 
            this.txtAirAtkYAdjLimit.Location = new System.Drawing.Point(104, 124);
            this.txtAirAtkYAdjLimit.Name = "txtAirAtkYAdjLimit";
            this.txtAirAtkYAdjLimit.Size = new System.Drawing.Size(62, 20);
            this.txtAirAtkYAdjLimit.TabIndex = 48;
            // 
            // lblAirAtkYAdjLimit
            // 
            this.lblAirAtkYAdjLimit.AutoSize = true;
            this.lblAirAtkYAdjLimit.Location = new System.Drawing.Point(6, 127);
            this.lblAirAtkYAdjLimit.Name = "lblAirAtkYAdjLimit";
            this.lblAirAtkYAdjLimit.Size = new System.Drawing.Size(92, 13);
            this.lblAirAtkYAdjLimit.TabIndex = 47;
            this.lblAirAtkYAdjLimit.Text = "Atk Y Adjust Limit:";
            // 
            // txtAirAtkXAdjLimit
            // 
            this.txtAirAtkXAdjLimit.Location = new System.Drawing.Point(512, 99);
            this.txtAirAtkXAdjLimit.Name = "txtAirAtkXAdjLimit";
            this.txtAirAtkXAdjLimit.Size = new System.Drawing.Size(64, 20);
            this.txtAirAtkXAdjLimit.TabIndex = 46;
            // 
            // lblAirAtkXAdjLimit
            // 
            this.lblAirAtkXAdjLimit.AutoSize = true;
            this.lblAirAtkXAdjLimit.Location = new System.Drawing.Point(417, 102);
            this.lblAirAtkXAdjLimit.Name = "lblAirAtkXAdjLimit";
            this.lblAirAtkXAdjLimit.Size = new System.Drawing.Size(89, 13);
            this.lblAirAtkXAdjLimit.TabIndex = 45;
            this.lblAirAtkXAdjLimit.Text = "Atk X Adjust Limit";
            // 
            // txtWallJmpDistanceLimit
            // 
            this.txtWallJmpDistanceLimit.Location = new System.Drawing.Point(344, 99);
            this.txtWallJmpDistanceLimit.Name = "txtWallJmpDistanceLimit";
            this.txtWallJmpDistanceLimit.Size = new System.Drawing.Size(67, 20);
            this.txtWallJmpDistanceLimit.TabIndex = 44;
            // 
            // lblWallJmpDistanceLimit
            // 
            this.lblWallJmpDistanceLimit.AutoSize = true;
            this.lblWallJmpDistanceLimit.Location = new System.Drawing.Point(216, 102);
            this.lblWallJmpDistanceLimit.Name = "lblWallJmpDistanceLimit";
            this.lblWallJmpDistanceLimit.Size = new System.Drawing.Size(122, 13);
            this.lblWallJmpDistanceLimit.TabIndex = 43;
            this.lblWallJmpDistanceLimit.Text = "Wall Jmp Distance Limit:";
            // 
            // txtWallJmpRecoilDistance
            // 
            this.txtWallJmpRecoilDistance.Location = new System.Drawing.Point(144, 99);
            this.txtWallJmpRecoilDistance.Name = "txtWallJmpRecoilDistance";
            this.txtWallJmpRecoilDistance.Size = new System.Drawing.Size(66, 20);
            this.txtWallJmpRecoilDistance.TabIndex = 42;
            // 
            // lblWallJmpRecoilDistance
            // 
            this.lblWallJmpRecoilDistance.AutoSize = true;
            this.lblWallJmpRecoilDistance.Location = new System.Drawing.Point(7, 102);
            this.lblWallJmpRecoilDistance.Name = "lblWallJmpRecoilDistance";
            this.lblWallJmpRecoilDistance.Size = new System.Drawing.Size(131, 13);
            this.lblWallJmpRecoilDistance.TabIndex = 41;
            this.lblWallJmpRecoilDistance.Text = "Wall Jmp Recoil Distance:";
            // 
            // btnUpdateP1
            // 
            this.btnUpdateP1.Enabled = false;
            this.btnUpdateP1.Location = new System.Drawing.Point(502, 410);
            this.btnUpdateP1.Name = "btnUpdateP1";
            this.btnUpdateP1.Size = new System.Drawing.Size(75, 23);
            this.btnUpdateP1.TabIndex = 62;
            this.btnUpdateP1.Text = "Update P1";
            this.btnUpdateP1.UseVisualStyleBackColor = true;
            this.btnUpdateP1.Click += new System.EventHandler(this.btnUpdateP1_Click);
            // 
            // btnSaveELF
            // 
            this.btnSaveELF.Enabled = false;
            this.btnSaveELF.Location = new System.Drawing.Point(421, 410);
            this.btnSaveELF.Name = "btnSaveELF";
            this.btnSaveELF.Size = new System.Drawing.Size(75, 23);
            this.btnSaveELF.TabIndex = 63;
            this.btnSaveELF.Text = "Save to ELF";
            this.btnSaveELF.UseVisualStyleBackColor = true;
            this.btnSaveELF.Click += new System.EventHandler(this.btnSaveELF_Click);
            // 
            // lblCharName
            // 
            this.lblCharName.AutoSize = true;
            this.lblCharName.Location = new System.Drawing.Point(7, 9);
            this.lblCharName.Name = "lblCharName";
            this.lblCharName.Size = new System.Drawing.Size(63, 13);
            this.lblCharName.TabIndex = 64;
            this.lblCharName.Text = "Char Name:";
            // 
            // lblCharName2
            // 
            this.lblCharName2.AutoSize = true;
            this.lblCharName2.BackColor = System.Drawing.SystemColors.Control;
            this.lblCharName2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.lblCharName2.Location = new System.Drawing.Point(68, 9);
            this.lblCharName2.Name = "lblCharName2";
            this.lblCharName2.Size = new System.Drawing.Size(13, 13);
            this.lblCharName2.TabIndex = 65;
            this.lblCharName2.Text = "?";
            // 
            // lblCharID
            // 
            this.lblCharID.AutoSize = true;
            this.lblCharID.Location = new System.Drawing.Point(512, 9);
            this.lblCharID.Name = "lblCharID";
            this.lblCharID.Size = new System.Drawing.Size(46, 13);
            this.lblCharID.TabIndex = 66;
            this.lblCharID.Text = "Char ID:";
            // 
            // lblCharID2
            // 
            this.lblCharID2.AutoSize = true;
            this.lblCharID2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.lblCharID2.Location = new System.Drawing.Point(556, 9);
            this.lblCharID2.Name = "lblCharID2";
            this.lblCharID2.Size = new System.Drawing.Size(13, 13);
            this.lblCharID2.TabIndex = 67;
            this.lblCharID2.Text = "?";
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(10, 410);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 69;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.VerifyOpenedELF);
            // 
            // GeneralParameters
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(585, 437);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.lblCharID2);
            this.Controls.Add(this.lblCharID);
            this.Controls.Add(this.lblCharName2);
            this.Controls.Add(this.lblCharName);
            this.Controls.Add(this.btnSaveELF);
            this.Controls.Add(this.btnUpdateP1);
            this.Controls.Add(this.grpMovementAirConfig);
            this.Controls.Add(this.grpMovementConfig);
            this.Controls.Add(this.grpGeneralConfig);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "GeneralParameters";
            this.Text = "Edit General Parameters:";
            this.grpMovementConfig.ResumeLayout(false);
            this.grpMovementConfig.PerformLayout();
            this.grpGeneralConfig.ResumeLayout(false);
            this.grpGeneralConfig.PerformLayout();
            this.grpMovementAirConfig.ResumeLayout(false);
            this.grpMovementAirConfig.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        public System.Windows.Forms.Label lblCharHeight;
        public System.Windows.Forms.TextBox txtCharHeight;
        public System.Windows.Forms.Label lblCharWidth;
        public System.Windows.Forms.TextBox txtCharWidth;
        public System.Windows.Forms.Label lblCharSpeed;
        public System.Windows.Forms.TextBox txtCharSpeed;
        public System.Windows.Forms.Label lblCharSlide;
        public System.Windows.Forms.TextBox txtCharSlide;
        public System.Windows.Forms.TextBox txtCharWeight;
        public System.Windows.Forms.Label lblCharWeight;
        public System.Windows.Forms.Label lblCharGravity;
        public System.Windows.Forms.TextBox txtCharGravity;
        public System.Windows.Forms.TextBox txtCharFirstJumpHeight;
        public System.Windows.Forms.Label lblCharFirstJumpHeight;
        public System.Windows.Forms.TextBox txtCharFirstJumpDelay;
        public System.Windows.Forms.Label lblCharFirstJumpDelay;
        public System.Windows.Forms.TextBox txtCharSpeedAir;
        public System.Windows.Forms.Label lblCharSpeedAir;
        public System.Windows.Forms.TextBox txtCharSecondJumpHeight;
        public System.Windows.Forms.Label lblCharSecondJumpHeight;
        public System.Windows.Forms.TextBox txtCharDashDuration;
        public System.Windows.Forms.Label lblCharDashDuration;
        public System.Windows.Forms.TextBox txtCharDashDelay;
        public System.Windows.Forms.Label lblCharDashDelay;
        public System.Windows.Forms.TextBox txtCharBackDashDistance;
        public System.Windows.Forms.Label lblCharBackDashDistance;
        public System.Windows.Forms.TextBox txtCharBackDashWeight;
        public System.Windows.Forms.Label lblCharBackDashWeight;
        public System.Windows.Forms.TextBox txtCharBackDashHeight;
        public System.Windows.Forms.Label lblCharBackDashHeight;
        public System.Windows.Forms.TextBox txtCharBackDashHeight2;
        public System.Windows.Forms.Label lblCharBackDashHeight2;
        public System.Windows.Forms.TextBox txtCharStrength;
        public System.Windows.Forms.Label lblCharStrength;
        public System.Windows.Forms.TextBox txtCharDefense;
        public System.Windows.Forms.Label lblCharDefense;
        public System.Windows.Forms.TextBox txtCharDamageKnockback;
        public System.Windows.Forms.Label lblCharDamageKnockback;
        public System.Windows.Forms.TextBox txtCharAttackKnockback;
        public System.Windows.Forms.Label lblCharAttackKnockback;
        public System.Windows.Forms.TextBox txtCharStatus;
        public System.Windows.Forms.Label lblCharStatus;
        public System.Windows.Forms.TextBox txtCharQuantityProjectiles;
        public System.Windows.Forms.Label lblCharQuantityProjectiles;
        public System.Windows.Forms.TextBox txtCharHealingMultiplier;
        public System.Windows.Forms.Label lblCharHealingMultiplier;
        public System.Windows.Forms.TextBox txtCharChakraRecoverySpeed;
        public System.Windows.Forms.Label lblCharChakraRecoverySpeed;
        public System.Windows.Forms.TextBox txtCharDashSpeed;
        public System.Windows.Forms.Label lblCharDashSpeed;
        public System.Windows.Forms.TextBox txtCharDashDistance;
        public System.Windows.Forms.Label lblCharDashDistance;
        public System.Windows.Forms.GroupBox grpMovementConfig;
        public System.Windows.Forms.GroupBox grpGeneralConfig;
        public System.Windows.Forms.GroupBox grpMovementAirConfig;
        public System.Windows.Forms.Button btnUpdateP1;
        public System.Windows.Forms.Label lblCharName;
        public System.Windows.Forms.Label lblCharName2;
        public System.Windows.Forms.Label lblCharID;
        public System.Windows.Forms.Label lblCharID2;
        public System.Windows.Forms.Button btnReset;
        public System.Windows.Forms.Button btnSaveELF;
        public System.Windows.Forms.Timer timer1;
        public System.Windows.Forms.Label lblRunningStartSpeed;
        public System.Windows.Forms.TextBox txtRunningStartSpeed;
        public System.Windows.Forms.TextBox txtWallJmpRecoilDistance;
        public System.Windows.Forms.Label lblWallJmpRecoilDistance;
        public System.Windows.Forms.TextBox txtWallJmpDistanceLimit;
        public System.Windows.Forms.Label lblWallJmpDistanceLimit;
        public System.Windows.Forms.TextBox txtAirAtkYAdjLimit;
        public System.Windows.Forms.Label lblAirAtkYAdjLimit;
        public System.Windows.Forms.TextBox txtAirAtkXAdjLimit;
        public System.Windows.Forms.Label lblAirAtkXAdjLimit;
    }
}