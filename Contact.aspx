<%@ Page Title="" Language="C#" MasterPageFile="~/Main1.master" AutoEventWireup="true" CodeFile="Contact.aspx.cs" Inherits="Contact" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" Runat="Server">
    <!-- Hero Section -->
<section class="bg-warning text-dark text-center py-4">
  <div class="container">
    <h3 class="mb-1">Contact Us</h3>
  </div>
</section>


<!-- Contact Section -->
<section class="py-5">
  <div class="container">
    <div class="row g-5">
      
      <!-- Contact Form -->
      <div class="col-lg-7">
        <h3>Get in Touch</h3>
        <form>
          <div class="mb-3">
            <label for="name" class="form-label">Your Name</label>
            <input type="text" class="form-control" id="name" placeholder="Your Name" required>
          </div>
          <div class="mb-3">
            <label for="email" class="form-label">Your Email</label>
            <input type="email" class="form-control" id="email" placeholder="you@example.com" required>
          </div>
          <div class="mb-3">
            <label for="subject" class="form-label">Subject</label>
            <input type="text" class="form-control" id="subject" placeholder="Subject">
          </div>
          <div class="mb-3">
            <label for="message" class="form-label">Message</label>
            <textarea class="form-control" id="message" rows="5" placeholder="Type your message here..." required></textarea>
          </div>
          <button type="submit" class="btn btn-success px-4">Send Message</button>
        </form>
      </div>

      <!-- Contact Info -->
      <div class="col-lg-5">
        <h3>Contact Information</h3>
        <ul class="list-unstyled">
          <li class="mb-3"><strong>Address:</strong> Bhubaneswar,Odisha</li>
          <li class="mb-3"><strong>Phone:</strong> <a href="tel:+91 8249323648">+91 8356784038</a></li>
          <li class="mb-3"><strong>Email:</strong> <a href="mailto:xyz@gmail.com">xyz@gmail.com</a></li>
        </ul>

        <!-- Optional Google Map -->
        <div class="ratio ratio-4x3">
          <iframe
    src="https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3726.469309820717!2d85.8129201150681!3d20.2959847133167!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x3a1909d2d5170aa5%3A0xfc580e2b68b33fa8!2sBhubaneswar!5e0!3m2!1sen!2sin!4v169____(your_timestamp_here)!5m2!1sen!2sin"
    style="border:0;"
    allowfullscreen=""
    loading="lazy">
  </iframe>
        </div>
      </div>

    </div>
  </div>
</section>
</asp:Content>

