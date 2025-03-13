// auth.js - Authentication handling for AJAX requests

$(document).ready(function() {
    // Set up AJAX request defaults
    $.ajaxSetup({
    headers:
        {
            'X-Requested-With': 'XMLHttpRequest'
        },
        error: function(xhr, textStatus, errorThrown) {
            if (xhr.status === 401)
            {
                // Unauthorized - redirect to login
                showAuthenticationModal();
            }
        }
    });

    function showAuthenticationModal()
    {
        // Create a bootstrap modal for authentication required
        if ($('#authRequiredModal').length === 0) {
            const modalHtml = `
                < div class= "modal fade" id = "authRequiredModal" tabindex = "-1" aria - labelledby = "authRequiredModalLabel" aria - hidden = "true" >
                    < div class= "modal-dialog" >
                        < div class= "modal-content" >
                            < div class= "modal-header bg-warning" >
                                < h5 class= "modal-title" id = "authRequiredModalLabel" > Authentication Required </ h5 >
                                < button type = "button" class= "btn-close" data - bs - dismiss = "modal" aria - label = "Close" ></ button >
                            </ div >
                            < div class= "modal-body" >
                                < p > You need to be logged in to access this feature.</p>
                                <p>Would you like to log in now?</p>
                            </div>
                            <div class= "modal-footer" >
                                < button type = "button" class= "btn btn-secondary" data - bs - dismiss = "modal" > Cancel </ button >
                                < a href = "/UserAccounts/Login" class= "btn btn-primary" > Login </ a >
                            </ div >
                        </ div >
                    </ div >
                </ div >
            `;
            
            $('body').append(modalHtml);
        }
        
        // Show the modal
        const authModal = new bootstrap.Modal(document.getElementById('authRequiredModal'));
authModal.show();
    }
});