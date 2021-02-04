using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;

namespace FoodOrderingSystem
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        SQLiteConnection con = new SQLiteConnection("Data Source=foodorderingsystem.db;");
        User user;
        List<Restaurant> restaurants = new List<Restaurant>();
        Dictionary<string, int> orderMenus = new Dictionary<string, int>();
        List<string> orderMenusNames = new List<string>();
        int restaurantID = -1, selectedMenuID = -1;


        // Running SQL Commands

        void RunSQLcommand(string SQLCommand)
        {
            con.Open();
            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand(SQLCommand, con))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            con.Close();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            panelWelcomeScreen.BringToFront();
            comboBoxRestaurantOrderStatus.SelectedIndex = 0;
        }


        // Creating buttons for each RESTAURANT 
        Button NewButton(string name, int id, Point location)
        {

            Button buton = new Button();
            buton.Width = 120;
            buton.Height = 50;
            buton.Location = location;
            buton.Font = new Font(Button.DefaultFont, FontStyle.Bold);
            buton.Text = name;
            buton.Name = id.ToString();
            buton.Click += Click;
            return buton;
        }

        // Creating buttons for each MENU
        Button NewButtonMenus(string name, int id, Point location)
        {
            Button buton = new Button();
            buton.Width = 120;
            buton.Height = 50;
            buton.Location = location;
            buton.Font = new Font(Button.DefaultFont, FontStyle.Bold);
            buton.Text = name;
            buton.Name = id.ToString();
            buton.Click += ClickMenu;
            return buton;
        }

        
        // Add menu items into the cart when clicked
        private void ClickMenu(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            checkedListBoxCart.Items.Add(btn.Text);
            int price = Convert.ToInt32(btn.Text.Split(' ')[btn.Text.Split(' ').Length-2]); // !!
            txtTotalFeeRestaurant.Text = (Convert.ToInt32(txtTotalFeeRestaurant.Text) + price).ToString();
        }

        
        // Opens restaurant menu when clicked
        private void Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            restaurantID = Convert.ToInt32(btn.Name);
            Point location = new Point(10, 63);
            int i = 0;
            con.Open();
            SQLiteCommand cmd = new SQLiteCommand("select menu_id,name,price FROM menus where restaurant_id=" + btn.Name + "", con);
            using (SQLiteDataReader dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    if (i == 0)
                    {
                        panelInsideRestaurant.Controls.Add(NewButtonMenus(dr["name"].ToString() + " - " + dr["price"].ToString() + " ₺", Convert.ToInt32(dr["menu_id"]), location));
                        i++;
                        continue;
                    }
                    if (i % 6 == 0)
                    {
                        location.X += 126;
                        location.Y = 10;
                        panelInsideRestaurant.Controls.Add(NewButtonMenus(dr["name"].ToString() + " - " + dr["price"].ToString() + " ₺", Convert.ToInt32(dr["menu_id"]), location));
                        i++;
                        continue;
                    }
                    location.Y += 56;
                    panelInsideRestaurant.Controls.Add(NewButtonMenus(dr["name"].ToString() + " - " + dr["price"].ToString() + " ₺", Convert.ToInt32(dr["menu_id"]), location));
                    i++;
                } 
            }
            con.Close();
            panelInsideRestaurant.BringToFront();
        }

        // Calls "Creating buttons for each RESTAURANT (NewButton) " with necessary parameters
        void CreateRestaurants()
        {
            Point location = new Point(10, 30);
            for (int i = 0; i < restaurants.Count; i++)
            {
                if(i == 0)
                {
                    panelUserScreen.Controls.Add(NewButton(restaurants[i].name,restaurants[i].id, location));
                    continue;
                }
                if(i%7 == 0)
                {
                    location.X += 126;
                    location.Y = 30;
                    panelUserScreen.Controls.Add(NewButton(restaurants[i].name, restaurants[i].id, location));
                    continue;
                }
                location.Y += 56;
                panelUserScreen.Controls.Add(NewButton(restaurants[i].name, restaurants[i].id, location));
            }
        }



        // Edits Orders dataGridView columns
        void RestaurantOrderDataGrid()
        {
            dataGridViewOrders.Columns[0].HeaderText = "MENU";
            dataGridViewOrders.Columns[1].HeaderText = "PRICE";
            dataGridViewOrders.Columns[2].HeaderText = "COUNT";
            dataGridViewOrders.Columns[3].HeaderText = "ADDRESS";
            dataGridViewOrders.Columns[4].HeaderText = "STATUS";
            dataGridViewOrders.Columns[0].Width = 45;
            dataGridViewOrders.Columns[1].Width = 42;
            dataGridViewOrders.Columns[2].Width = 40;
            dataGridViewOrders.Columns[3].Width = 140;
            dataGridViewOrders.Columns[4].Width = 75;
        }

        // Edits Menu dataGridView columns
        void RestaurantMenuDataGrid()
        {
            dataGridViewRestaurantMenus.Columns[0].HeaderText = "ID";
            dataGridViewRestaurantMenus.Columns[1].HeaderText = "MENU";
            dataGridViewRestaurantMenus.Columns[2].HeaderText = "PRICE";
            dataGridViewRestaurantMenus.Columns[0].Width = 20;
            dataGridViewRestaurantMenus.Columns[1].Width = 60;
            dataGridViewRestaurantMenus.Columns[2].Width = 50;
        }

        private void linkLabelRegister_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            panelRegister.BringToFront();
        }

        // Back to Login Screen
        private void linkLabelRegisterGoBack_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            panelWelcomeScreen.BringToFront();
            for (int i = 0; i < groupBox1.Controls.Count; i++)
            {
                Control item = groupBox1.Controls[i];
                if(item is TextBox)
                {
                    item.Text = "";
                }
            }
            for (int i = 0; i < groupBox2.Controls.Count; i++)
            {
                Control item = groupBox2.Controls[i];
                if(item is TextBox)
                {
                    item.Text = "";
                }
            }
        }

        // Creating an SQL USER DATA with informations
        private void btnAddUser_Click(object sender, EventArgs e)
        {
            if(String.IsNullOrEmpty(txtUserTC.Text) || String.IsNullOrEmpty(txtUserPersonalName.Text) || String.IsNullOrEmpty(txtUserAddress.Text) || String.IsNullOrEmpty(txtUserName.Text) || String.IsNullOrEmpty(txtUserPassword.Text) || String.IsNullOrEmpty(txtUserSurname.Text) || String.IsNullOrEmpty(txtUserPhone.Text))
            {
                MessageBox.Show("Don't leave blank component.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            RunSQLcommand("INSERT into users VALUES('" + txtUserTC.Text + "','" + txtUserName.Text + "','" + txtUserPassword.Text + "','" + txtUserPersonalName.Text + "','" + txtUserSurname.Text + "','" + txtUserPhone.Text + "','" + txtUserAddress.Text + "')");
            txtUserTC.Text = txtUserName.Text = txtUserPassword.Text = txtUserPersonalName.Text = txtUserSurname.Text = txtUserPhone.Text = txtUserAddress.Text = "";
            MessageBox.Show("Registration Successful.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Creating an SQL Restaurant DATA with informations
        private void btnAddRestaurant_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtRestaurantName.Text) || String.IsNullOrEmpty(txtRestaurantUserName.Text) || String.IsNullOrEmpty(txtRestaurantPassword.Text) || String.IsNullOrEmpty(txtRestaurantPhone.Text))
            {
                MessageBox.Show("Don't leave blank component.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            RunSQLcommand("INSERT into restaurants(user_name,password,name,phone) VALUES('" + txtRestaurantUserName.Text + "','" + txtRestaurantPassword.Text + "','" + txtRestaurantName.Text + "','" + txtRestaurantPhone.Text + "')");
            txtRestaurantName.Text = txtRestaurantUserName.Text = txtRestaurantPassword.Text = txtRestaurantPhone.Text = "";
            MessageBox.Show("Registration Successful.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Authentication Process - Login USER ( If Username and Password are OK)
        // Creating objects for all restaurants and users
        // Calls createRestaurant function, gets informations from Restaurant class
        private void btnLoginUser_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtLoginUserName.Text) || String.IsNullOrEmpty(txtLoginPassword.Text))
            {
                MessageBox.Show("Don't leave blank component.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            con.Open();
            using (SQLiteCommand sql = new SQLiteCommand("Select * FROM users where user_name='" + txtLoginUserName.Text + "' and password='" + txtLoginPassword.Text + "'", con))
            {
                using (SQLiteDataReader dr = sql.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        MessageBox.Show("Login successfully.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        panelUserScreen.BringToFront();
                        txtLoginUserName.Text = txtLoginPassword.Text = "";
                        user = new User(dr["user_name"].ToString(), dr["password"].ToString(), dr["tc_id"].ToString(), dr["name"].ToString(), dr["surname"].ToString(), dr["phone"].ToString(), dr["address"].ToString());
                        restaurants.Clear();

                        for (int i = 0; i < panelUserScreen.Controls.Count; i++)
                        {
                            Control item = panelUserScreen.Controls[i];
                            for (int k = 1; k < 50; k++)
                            {
                                string a = k.ToString();
                                if (item.Name.Equals(a))
                                {
                                    panelUserScreen.Controls.Remove(item);
                                    i--;
                                    break;
                                }
                            }
                        }

                        SQLiteCommand sql1 = new SQLiteCommand("Select * FROM restaurants", con);
                        using (SQLiteDataReader dr1 = sql1.ExecuteReader())
                        {
                            while (dr1.Read())
                                restaurants.Add(new Restaurant(Convert.ToInt32(dr1["id"]), dr1["name"].ToString(), dr1["phone"].ToString()));
                        }


                        SQLiteCommand sql2 = new SQLiteCommand("Select * FROM menus", con);
                        using (SQLiteDataReader dr2 = sql2.ExecuteReader())
                        {
                            while (dr2.Read())
                            {
                                foreach (var item in restaurants)
                                    if (item.id == Convert.ToInt32(dr2["restaurant_id"]))
                                        item.AddMenu(Convert.ToInt32(dr2["menu_id"]), dr2["name"].ToString(), Convert.ToInt32(dr2["price"]));
                            }
                        }
                        CreateRestaurants();
                    }
                    else
                        MessageBox.Show("Invalid username or password.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    con.Close();
                }
            }
                
        }

        // Authentication Process - Login RESTAURANT ( If Username and Password are OK)
        // Updating Menu and Order DataGridViews with SQL Datas 
        private void btnLoginRestaurant_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtLoginUserName.Text) || String.IsNullOrEmpty(txtLoginPassword.Text))
            {
                MessageBox.Show("Don't leave blank component.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            con.Open();
            SQLiteCommand sql = new SQLiteCommand("Select * FROM restaurants where user_name='" + txtLoginUserName.Text + "' and password='" + txtLoginPassword.Text + "'", con);
            using (SQLiteDataReader dr = sql.ExecuteReader())
            {
                if (dr.Read())
                {
                    MessageBox.Show("Login successfully.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    panelRestaurantScreen.BringToFront();
                    txtLoginUserName.Text = txtLoginPassword.Text = "";
                    restaurantID = Convert.ToInt32(dr["id"]);

                    dataGridViewOrders.DataSource = "";
                    SQLiteDataAdapter da = new SQLiteDataAdapter("SELECT M.name,M.price,S.number,K.address,S.order_status FROM orders as S,users as K,menus as M where S.user_tc = K.tc_id and M.menu_id = S.menu_id and S.restaurant_id = M.restaurant_id and S.restaurant_id=" + restaurantID + "", con);
                    using (DataSet ds = new DataSet())
                    {
                        da.Fill(ds);
                        dataGridViewOrders.DataSource = ds.Tables[0];
                    }
                    RestaurantOrderDataGrid();

                    comboBoxRestaurantOrder.Items.Clear();
                    comboBoxRestaurantOrder.Text = "";
                    SQLiteCommand cmd = new SQLiteCommand("SELECT S.id,M.restaurant_id,M.menu_id,M.name,M.price,K.address,S.order_status FROM orders as S,users as K,menus as M where S.user_tc = K.tc_id and M.menu_id = S.menu_id and S.restaurant_id = M.restaurant_id and S.restaurant_id=" + restaurantID + "", con);
                    using (SQLiteDataReader dr1 = cmd.ExecuteReader())
                    {
                        while (dr1.Read())
                        {
                            comboBoxRestaurantOrder.Items.Add(dr1["id"].ToString() + " " + dr1["name"].ToString() + " " + dr1["price"].ToString() + " " + dr1["address"].ToString() + " " + dr1["order_status"].ToString());
                            comboBoxRestaurantOrder.SelectedIndex = 0;
                        }
                    }

                    dataGridViewRestaurantMenus.DataSource = "";
                    SQLiteDataAdapter da1 = new SQLiteDataAdapter("SELECT menu_id,name,price FROM menus where restaurant_id=" + restaurantID + "", con);
                    using (DataSet ds = new DataSet())
                    {
                        da1.Fill(ds);
                        dataGridViewRestaurantMenus.DataSource = ds.Tables[0];
                    }
                    RestaurantMenuDataGrid();
                }
                else
                    MessageBox.Show("Invalid username or password.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            con.Close();
        }

        // Combobox Order Status Update Current
        private void btnRestaurantOrderUpdate_Click(object sender, EventArgs e)
        {
            if(comboBoxRestaurantOrder.SelectedIndex == -1)
            {
                MessageBox.Show("Don't leave blank component.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            RunSQLcommand("UPDATE orders SET order_status='" + comboBoxRestaurantOrderStatus.SelectedItem.ToString() + "' where id=" + comboBoxRestaurantOrder.SelectedItem.ToString().Split(' ')[0]);
            
            dataGridViewOrders.DataSource = "";
            con.Open();
            SQLiteDataAdapter da = new SQLiteDataAdapter("SELECT M.name,M.price,S.number,K.address,S.order_status FROM orders as S,users as K,menus as M where S.user_tc = K.tc_id and M.menu_id = S.menu_id and S.restaurant_id = M.restaurant_id and S.restaurant_id=" + restaurantID + "", con);
            DataSet ds = new DataSet();
            da.Fill(ds);
            dataGridViewOrders.DataSource = ds.Tables[0];
            con.Close();
            RestaurantOrderDataGrid();
        }

        // Adding Menu to the Database
        private void btnRestaurantMenuAdd_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtRestaurantMenuName.Text) || String.IsNullOrEmpty(txtRestoranMenuFiyati.Text))
            {
                MessageBox.Show("Don't leave blank component.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int menu_id = 1;
            con.Open();
            SQLiteCommand cmd = new SQLiteCommand("select max(menu_id) FROM menus where restaurant_id=" + restaurantID + "",con);
            using (SQLiteDataReader dr = cmd.ExecuteReader())
            {
                if (dr.Read())
                    if(dr["max(menu_id)"] != DBNull.Value)
                        menu_id += Convert.ToInt32(dr["max(menu_id)"]);
            }

            using (SQLiteCommand cmd1 = new SQLiteCommand("INSERT INTO menus VALUES(" + restaurantID + "," + menu_id + ",'" + txtRestaurantMenuName.Text + "'," + txtRestoranMenuFiyati.Text + ")", con))
            { cmd1.ExecuteNonQuery(); }

            dataGridViewRestaurantMenus.DataSource = "";
            SQLiteDataAdapter da1 = new SQLiteDataAdapter("SELECT menu_id,name,price FROM menus where restaurant_id=" + restaurantID + "", con);
            using (DataSet ds = new DataSet())
            {
                da1.Fill(ds);
                dataGridViewRestaurantMenus.DataSource = ds.Tables[0];
            }
            RestaurantMenuDataGrid();
            txtRestaurantMenuName.Text = txtRestoranMenuFiyati.Text = "";
            con.Close();
        }

        // Menu DataGridBiew Informations to the TextBoxes
        private void dataGridViewRestaurantMenus_SelectionChanged(object sender, EventArgs e)
        {
            if(dataGridViewRestaurantMenus.CurrentRow != null && dataGridViewRestaurantMenus.CurrentRow.Cells[0].Value.ToString() != "")
            {
                selectedMenuID = Convert.ToInt32(dataGridViewRestaurantMenus.CurrentRow.Cells[0].Value.ToString());
                txtRestaurantUpdateMenuName.Text = dataGridViewRestaurantMenus.CurrentRow.Cells[1].Value.ToString();
                txtRestaurantUpdateMenuPrice.Text = dataGridViewRestaurantMenus.CurrentRow.Cells[2].Value.ToString();
            }
        }

        // Delete Menu (From Database)
        private void btnRestaurantMenuDelete_Click(object sender, EventArgs e)
        {
            int cevap = Convert.ToInt32(MessageBox.Show("Menu number "+ selectedMenuID + " will be deleted, are you sure?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning));
            if (cevap == 6)
            {
                RunSQLcommand("DELETE FROM menus where restaurant_id=" + restaurantID + " and menu_id=" + selectedMenuID + "");
                dataGridViewRestaurantMenus.DataSource = "";
                SQLiteDataAdapter da1 = new SQLiteDataAdapter("SELECT menu_id,name,price FROM menus where restaurant_id=" + restaurantID + "", con);
                using (DataSet ds = new DataSet())
                {
                    da1.Fill(ds);
                    dataGridViewRestaurantMenus.DataSource = ds.Tables[0];
                }
                RestaurantMenuDataGrid();
            }
        }

        // Closing Restaurant and goes to WelcomePage
        private void btnRestaurantClose_Click(object sender, EventArgs e)
        {
            panelWelcomeScreen.BringToFront();
        }

        // Warning MessageBox in Order!
        private void btnCloseMenus_Click(object sender, EventArgs e)
        {
            if(checkedListBoxCart.Items.Count > 0)
            {
                int cevap = Convert.ToInt32(MessageBox.Show("Cart gonna be empty, are you sure ?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning));
                if (cevap != 6)
                    return;
            }
            for (int i = 0; i < panelInsideRestaurant.Controls.Count; i++)
            {
                Control item = panelInsideRestaurant.Controls[i];
                for (int k = 1; k < 50; k++)
                {
                    string a = k.ToString();
                    if (item.Name.Equals(a))
                    {   
                        panelInsideRestaurant.Controls.Remove(item);
                        i--;
                        break;
                    }
                }
            }
            txtTotalFeeRestaurant.Text = "0";
            checkedListBoxCart.Items.Clear();
            panelUserScreen.BringToFront();
        }

        // Delete Selected Items from Cart
        private void btnRemoveFromCart_Click(object sender, EventArgs e)
        {
            if(checkedListBoxCart.CheckedItems.Count > 0)
            {
                for (int i = checkedListBoxCart.CheckedItems.Count-1; i >= 0; i--)
                {
                    int fee = Convert.ToInt32(checkedListBoxCart.CheckedItems[i].ToString().Split(' ')[checkedListBoxCart.CheckedItems[i].ToString().Split(' ').Length-2]);
                    txtTotalFeeRestaurant.Text = (Convert.ToInt32(txtTotalFeeRestaurant.Text) - fee).ToString();
                    checkedListBoxCart.Items.Remove(checkedListBoxCart.CheckedItems[i]);
                }
                for (int i = 0; i < checkedListBoxCart.Items.Count; i++)
                    checkedListBoxCart.SetItemChecked(i,false);
            }
        }

        // Counts Menu Number when Order
        void FindCount(string menu)
        {
            if (orderMenus.ContainsKey(menu))
                return;
            int count = 0;
            for (int i = 0; i < checkedListBoxCart.Items.Count; i++)
                if (checkedListBoxCart.Items[i].Equals(menu))
                    count++;
            orderMenus.Add(menu, count);
            orderMenusNames.Add(menu);
        }

        // Find Restaurant from Restaurant Object
        // Finds into Carts from Selected Restaurant Menus and Order (Adds SQL to the orders Table)
        private void btnOrder_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < checkedListBoxCart.Items.Count; i++)
                FindCount(checkedListBoxCart.Items[i].ToString());
            bool inserted = false;
            for (int i = 0; i < restaurants.Count; i++)
            {
                if (restaurants[i].id == restaurantID)
                {
                    for (int k = 0; k < orderMenusNames.Count; k++)
                    {
                        for (int j = 0; j < restaurants[i].menus.Count; j++)
                        {
                            string menu = orderMenusNames[k].ToString().Split(' ')[0] + " " + orderMenusNames[k].ToString().Split(' ')[1];
                            if (restaurants[i].menus[j].name == menu && restaurants[i].menus[j].price == Convert.ToInt32(orderMenusNames[k].ToString().Split(' ')[orderMenusNames[k].ToString().Split(' ').Length-2]))
                            {
                                RunSQLcommand("INSERT into orders(user_tc,restaurant_id,menu_id,number,order_status) VALUES(" + user.tcID + "," + restaurantID + ",'" + restaurants[i].menus[j].id + "'," + orderMenus[checkedListBoxCart.Items[k].ToString()] + ",'Preparing')");
                                inserted = true;
                            }
                        }
                    }
                }
            }
            if(inserted)
            {
                checkedListBoxCart.Items.Clear();
                txtTotalFeeRestaurant.Text = "0";
                MessageBox.Show("Order created.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Logout and Welcome Screen
        private void btnUserExit_Click(object sender, EventArgs e)
        {
            panelWelcomeScreen.BringToFront();
        }

        // Update Menu informations which selected from DataGriedView (From TextBox) 
        private void btnRestaurantMenuUpdate_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtRestaurantUpdateMenuName.Text) || String.IsNullOrEmpty(txtRestaurantUpdateMenuPrice.Text))
            {
                MessageBox.Show("Don't leave blank component.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            RunSQLcommand("UPDATE menus SET name='" + txtRestaurantUpdateMenuName.Text + "',price=" + txtRestaurantUpdateMenuPrice.Text + " WHERE restaurant_id=" + restaurantID + " and menu_id=" + selectedMenuID + "");
            dataGridViewRestaurantMenus.DataSource = "";
            SQLiteDataAdapter da1 = new SQLiteDataAdapter("SELECT menu_id,name,price FROM menus where restaurant_id=" + restaurantID + "", con);
            using (DataSet ds = new DataSet())
            {
                da1.Fill(ds);
                dataGridViewRestaurantMenus.DataSource = ds.Tables[0];
            }
            RestaurantMenuDataGrid();
        }
    }
}
